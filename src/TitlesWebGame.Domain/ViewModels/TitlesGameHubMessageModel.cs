using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.ViewModels
{
    public class TitlesGameHubMessageModel
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public GameHubMessageType MessageType { get; set; }
        public object AppendedObject { get; set; }
    }
}