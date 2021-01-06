using Microsoft.Extensions.Configuration;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Api.Infrastructure.Repositories
{
    public class GameRoundInfoRepository : Repository<GameRoundInfo>
    {
        public GameRoundInfoRepository(string tableName, IConfiguration configuration) : base(tableName, configuration)
        {
            
        }
        
    }
}