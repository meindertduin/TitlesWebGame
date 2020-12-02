namespace TitlesWebGame.Api.Services
{
    public interface IGameConnectionGroupsManager
    {
        string CreateSession();
        bool DeleteSession(string roomKey);
    }
}