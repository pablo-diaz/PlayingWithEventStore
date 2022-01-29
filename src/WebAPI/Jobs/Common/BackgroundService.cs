using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace WebAPI.Jobs.Common
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        protected CancellationTokenSource _stoppingCts;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            this._stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await ExecuteAsync(_stoppingCts.Token);
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _stoppingCts?.Cancel();
        }
    }
}
