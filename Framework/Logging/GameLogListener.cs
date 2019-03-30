namespace NekoClient.Logging
{
    public class GameLogListener : ILogListener
    {
        public void LogMessage(string source, string message, LogLevel level)
        {
            VRC.Core.Logger.Log("[<color=#348BD5>" + source + "</color>] " + message + "\n");
        }

        public bool WantsFilteredMessages { get { return true; } }
    }
}