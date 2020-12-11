using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.WebUi.Services
{
    public class TitlesCategoryEmojiFactory : ITitlesCategoryEmojiFactory
    {
        public string GetUniCodeEmojiString(TitleCategory titleCategory) => 
            titleCategory switch
            {
                TitleCategory.Scientist => "&#129514;",
                TitleCategory.Artist => "&#128396;&#65039;",
                TitleCategory.Comedian => "&#127917;",
                TitleCategory.Athlete => "&#9917;",
                TitleCategory.Musician => "&#127932;",
                _ => "&#129514;",
            };
    }
}