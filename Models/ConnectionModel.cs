using System.Reflection;
using System.Text.Json.Serialization;

namespace RemoteConnection.Models
{
    public class ConnectionModel
    {
        private string? _hostname;
        private string? _username;
        private string? _password;

        public string? Hostname
        {
            get => _hostname;
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Hostname cannot be empty", nameof(Hostname));
                }
                else
                {
                    _hostname = value;
                }
            }
        }

        public string? Username
        {
            get => _username;
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Username cannot be empty", nameof(Username));
                }
                else
                {
                    _username = value;
                }
            }
        }

        public string? Password
        {
            get
            {
                string? maskedPassword = new string('*', _password?.Length ?? 0);
                return maskedPassword;
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Password cannot be empty", nameof(Password));
                }
                else
                {
                    _password = value;
                }
            }
        }

        internal string GetRealPassword()
        {
            return _password;
        }
    }
}