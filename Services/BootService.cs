using RemoteConnection.Models;
using RemoteConnection.Services;
using System;
using System.Text;


namespace RemoteConnection.Services
{
    public class BootService
    {
        static ConnectionBuilder? _connectionBuilder;
        static CommandService? _commandService;
        private static string? _hostname;
        private static string? _username;

        private static string DisplayLogo()
        {
            return @"
             ██████╗ ██╗ ██╗     ███████╗███████╗██╗  ██╗
            ██╔════╝████████╗    ██╔════╝██╔════╝██║  ██║
            ██║     ╚██╔═██╔╝    ███████╗███████╗███████║
            ██║     ████████╗    ╚════██║╚════██║██╔══██║
            ╚██████╗╚██╔═██╔╝    ███████║███████║██║  ██║
             ╚═════╝ ╚═╝ ╚═╝     ╚══════╝╚══════╝╚═╝  ╚═╝
                                                         ";
        }


        public static void GetConnectionInput()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(DisplayLogo() + "\n\n");
            Console.ResetColor();

            Console.WriteLine("Formatting: <host>@<username>");
            Console.Write("Enter Connection Details: ");
            string? input = Console.ReadLine();

            string[]? parts = input?.Split('@', 2);
            string host = string.Empty;
            string username = string.Empty;

            if (parts?.Length == 2)
            {
                host = parts[0].Trim();
                username = parts[1].Trim();
            }

            string? password = MaskPassword("Enter Password: ");

            _connectionBuilder = new ConnectionBuilder
            (
                host ?? string.Empty,
                username ?? string.Empty,
                password ?? string.Empty
            );

            _connectionBuilder.Connect();
            if (_connectionBuilder.GetSShClient() is { IsConnected: true } sshClient)
            {
                _commandService = new CommandService(sshClient);
                _hostname = _connectionBuilder.GetConnectionModel()?.Hostname ?? string.Empty;
                _username = _connectionBuilder.GetConnectionModel()?.Username ?? string.Empty;
                CommandLoop();
            }
            else
            {
                Console.WriteLine("Failed to connect. Please check your credentials and try again.");
            }
        }

        private static void CommandLoop()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{_hostname}@{_username}>: ");
                Console.ResetColor();
                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                var input = Console.ReadLine();
                Console.SetCursorPosition(cursorLeft, cursorTop);
                if (string.IsNullOrEmpty(input))
                {
                    break;
                }

                var command = new CommandModel { Command = input, Timeout = TimeSpan.FromSeconds(10) };
                var output = _commandService?.ExecuteCommand(command);
                Console.WriteLine(output);
            }
        }

        private static string MaskPassword(string prompt)
        {
            Console.Write(prompt);
            StringBuilder maskedPassword = new StringBuilder();
            ConsoleKeyInfo KeyInfo;

            do
            {
                KeyInfo = Console.ReadKey(true);
                if (KeyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (KeyInfo.Key == ConsoleKey.Backspace)
                {
                    if (maskedPassword.Length > 0)
                    {
                        maskedPassword.Remove(maskedPassword.Length - 1, 1);
                        Console.Write("\b\b");
                    }
                }
                else if (!char.IsControl(KeyInfo.KeyChar))
                {
                    maskedPassword.Append(KeyInfo.KeyChar);
                    Console.Write("*");
                }
            }
            while (true);
            Console.WriteLine();
            return maskedPassword.ToString();
        }
    }
}