using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TitlesWebGame.Api.Services
{
    public class SessionCleanupService : IHostedService, IDisposable
    {
        private Timer _timer;
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CleanupEmptySessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        private void CleanupEmptySessions(object state)
        {
            GameSessionManager.CleanUpEmptySessions();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}