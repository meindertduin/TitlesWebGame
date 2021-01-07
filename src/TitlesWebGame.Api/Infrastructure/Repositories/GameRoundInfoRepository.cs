using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return MapGameRoundInfo(result);
        }

        public async Task<List<GameRoundInfo>> GetRandomRounds(int[] categories, int amountPerCategory)
        {
            using var connection = CreateConnection();
            var queryString =
                new StringBuilder(
                    $"SELECT * FROM {TableName} WHERE Id IN (SELECT Id FROM {TableName} WHERE GameRoundsType IN (");
            foreach (var category in categories)
            {
                queryString.Append($"{category},");
            }

            queryString
                .Remove(queryString.Length -1, 1)
                .Append($") ORDER BY RANDOM())");


            var query = queryString.ToString();
            var result = await connection.QueryAsync(query);

            var gameRounds = new List<GameRoundInfo>();
            foreach (var res in result)
            {
                var mapResult = MapGameRoundInfo(res);
                if (mapResult != null)
                {
                    gameRounds.Add(mapResult);
                }
            }

            return gameRounds;
        }
        
        private GameRoundInfo MapGameRoundInfo(dynamic result){
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
        Task<List<GameRoundInfo>> GetRandomRounds(int[] categories, int amountPerCategory);
    }
}