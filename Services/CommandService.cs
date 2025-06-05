using System;
using System.Text;
using RemoteConnection.Models;
using Renci.SshNet;
using System.Threading.Tasks;

namespace RemoteConnection.Services
{
    public class CommandService : IDisposable
    {
        private readonly SshClient _sshClient;
        private readonly ShellStream _shellStream;

        // custom prompt to identify command completion
        private readonly string _promptMaker = ">>COMMAND_FINISHED<<";

        public CommandService(SshClient sshClient)
        {
            _sshClient = sshClient ?? throw new ArgumentException(nameof(sshClient), "SSH Client cannot be null");
            if(!_sshClient.IsConnected)
            {
                _sshClient.Connect();
            }
            _shellStream = sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
            SetCustomPrompt();
        }

        public string ExecuteCommand(CommandModel commandModel)
        {
            // prepare command text with sudo if required
            string cmdText = commandModel.IsSudo ? $"sudo {commandModel.Command}" : commandModel.Command;

            if(cmdText == "exit")
            {
                Dispose();
                Environment.Exit(0);
            }    
            _shellStream.WriteLine(cmdText);
            return ReadUntilPrompt(commandModel.Timeout);
        }

        private void SetCustomPrompt()
        {
            // PS1 is the evironment variable that defines the shell prompt in bash
            // this allows us to identify when a command has finished executing
            _shellStream.WriteLine($"PS1='{_promptMaker}'");
            ReadUntilPrompt(); // read until prompt to clear output buffer
        }

        public void Dispose()
        {
            _shellStream?.Dispose();
            if(_sshClient.IsConnected)
            {
                _sshClient.Disconnect();
            }
            _sshClient?.Dispose();
        }

        private string ReadUntilPrompt(TimeSpan? timeout = null)
        {
            // use stringbuilder for efficient string concatenation of incoming data
            var output = new StringBuilder();
            // if the commands output doesnt include the prompt maker within 10 seconds, it stops the loop and returns the output so far.
            var deadline = DateTime.Now + (timeout ?? TimeSpan.FromSeconds(10));

            while(DateTime.Now < deadline)
            {
                if(_shellStream.DataAvailable)
                {
                    string data = _shellStream.Read();
                    output.Append(data);

                    if(output.ToString().Contains(_promptMaker))
                    {
                        return output.ToString().Replace(_promptMaker, "").Trim();
                    }
                }
                else
                {
                    Task.Delay(100).Wait();
                }
            }

            return output.ToString().Trim();
        }
    }
}