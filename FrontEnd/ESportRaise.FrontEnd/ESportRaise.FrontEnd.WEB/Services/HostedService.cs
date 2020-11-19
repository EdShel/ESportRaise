using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ESportRaise.FrontEnd.WEB.Services
{
    public abstract class HostedService : IHostedService
    {
        private Task executingTask;

        private CancellationTokenSource cancellationTokenSource;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            executingTask = ExecuteAsync(cancellationTokenSource.Token);

            return executingTask.IsCompleted ? executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (executingTask == null)
            {
                return;
            }

            cancellationTokenSource.Cancel();

            await Task.WhenAny(executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
