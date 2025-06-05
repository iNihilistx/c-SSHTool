using System;
using RemoteConnection.Models;
using Renci.SshNet;

namespace RemoteConnection.Services
{
    public class ConnectionBuilder
    {
        private ConnectionModel? _connectionModel;
        private SshClient? _sshClient;

        public ConnectionBuilder(string host, string username, string password)
        {
            _connectionModel = new ConnectionModel
            {
                Hostname = host,
                Username = username,
                Password = password
            };
        }

        public void Connect()
        {
            _sshClient = new SshClient(_connectionModel?.Hostname ?? string.Empty, _connectionModel?.Username ?? string.Empty, _connectionModel.GetRealPassword());
            try
            {
                _sshClient.Connect();
                Console.WriteLine("Connection established successfully.");
                Console.Clear();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to connect: {ex.Message}");
            }
        }

        public ConnectionModel? GetConnectionModel() => _connectionModel;
        public SshClient? GetSShClient() => _sshClient;
    }
}