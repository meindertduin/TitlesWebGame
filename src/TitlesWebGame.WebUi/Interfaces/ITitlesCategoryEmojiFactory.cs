using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.WebUi.Services
{
    public interface ITitlesCategoryEmojiFactory
    {
        string GetUniCodeEmojiString(TitleCategory titleCategory);
    }
}