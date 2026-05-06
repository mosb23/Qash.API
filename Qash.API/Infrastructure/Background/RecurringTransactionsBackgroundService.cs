using MediatR;
using Qash.API.Features.RecurringTransactions.Commands;

namespace Qash.API.Infrastructure.Background;

public class RecurringTransactionsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecurringTransactionsBackgroundService> _logger;

    public RecurringTransactionsBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<RecurringTransactionsBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(new ProcessDueRecurringTransactionsCommand(), stoppingToken);

                if (result.GeneratedCount > 0)
                {
                    _logger.LogInformation(
                        "Recurring job generated {Count} transaction(s).",
                        result.GeneratedCount);
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Recurring job issue: {Error}", error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recurring transaction job failed.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
