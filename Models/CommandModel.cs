namespace RemoteConnection.Models
{
    public class CommandModel
    {
        public string? Command { get; set; }
        public TimeSpan Timeout { get; set; }
        public bool IsSudo { get; set; }
    }
}