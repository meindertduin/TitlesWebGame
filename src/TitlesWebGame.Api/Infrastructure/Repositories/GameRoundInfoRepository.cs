using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Api.Infrastructure.Repositories
{
    public class GameRoundInfoRepository : Repository<GameRoundInfo>, IGameRoundInfoRepository
    {
        private const string TableName = "GameRoundInfo";
        public GameRoundInfoRepository(IConfiguration configuration) : base(TableName, configuration)
        {
            
        }

        public override async Task InsertAsync(GameRoundInfo t)
        {
            using var connection = CreateConnection();
            if (t is CompetitiveArtistVotingRoundInfo paintingRoundInfo)
            {
                var insertQuery = GenerateDiscriminatedInsertQuery<CompetitiveArtistVotingRoundInfo>();
                await connection.ExecuteAsync(insertQuery, paintingRoundInfo);
            }
        }
        
        public override async Task<GameRoundInfo> GetAsync(int id)
        {
            using var connection = CreateConnection();
            var result =
                await connection.QuerySingleOrDefaultAsync($"SELECT * FROM {TableName} WHERE Id=@Id", new {Id = id});
            
            switch (result.Discriminator)
            {
                case nameof(MultipleChoiceRoundInfo):
                    return Slapper.AutoMapper.Map<MultipleChoiceRoundInfo>(result);
                case nameof(CompetitiveArtistVotingRoundInfo):
                    return Slapper.AutoMapper.Map<CompetitiveArtistRoundInfo>(result);
                default:
                    return null;
            }
        }
    }

    public interface IGameRoundInfoRepository
    {
        Task InsertAsync(GameRoundInfo t);
        Task<GameRoundInfo> GetAsync(int id);
    }
}