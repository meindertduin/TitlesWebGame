using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class RoundInfoDeserializer
    {
        public GameRoundInfo DeserializeModel(string jsonString, GameRoundsType roundsType) =>
            roundsType switch
            {
                GameRoundsType.MultipleChoiceRound =>
                    JsonConvert.DeserializeObject<MultipleChoiceRoundInfo>(jsonString),
                GameRoundsType.CompetitiveArtistRound =>
                    JsonConvert.DeserializeObject<CompetitiveArtistRoundInfo>(jsonString),
                _ => null,
            };

        public GameRoundInfoViewModel DeserializeViewModel(string jsonString, GameRoundsType roundsType) =>
            roundsType switch
            {
                GameRoundsType.MultipleChoiceRound =>
                    JsonConvert.DeserializeObject<MultipleChoiceRoundInfoViewModel>(jsonString),
                GameRoundsType.CanvasPaintingRound =>
                    JsonConvert.DeserializeObject<CanvasPaintingRoundInfoViewModel>(jsonString),
                GameRoundsType.CompetitiveArtistVotingRound =>
                    JsonConvert.DeserializeObject<CompetitiveArtistVotingRoundInfoViewModel>(jsonString),
                GameRoundsType.CompetitiveArtistReviewRound =>
                    JsonConvert.DeserializeObject<CompetitiveArtistReviewRoundInfoViewModel>(jsonString),
                GameRoundsType.CompetitiveArtistUploadRound =>
                    JsonConvert.DeserializeObject<CompetitiveArtistUploadRoundInfoViewModel>(jsonString),
                _ => null,
            };
    }
}