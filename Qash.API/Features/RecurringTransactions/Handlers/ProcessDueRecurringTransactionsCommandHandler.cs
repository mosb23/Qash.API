using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Features.RecurringTransactions.Commands;
using Qash.API.Features.Transactions.Commands;
using Qash.API.Infrastructure.Data;
using Qash.API.Infrastructure.Scheduling;

namespace Qash.API.Features.RecurringTransactions.Handlers;

public class ProcessDueRecurringTransactionsCommandHandler : IRequestHandler<ProcessDueRecurringTransactionsCommand, ProcessDueRecurringTransactionsResult>
{
    private const int MaxCatchUpPerSchedule = 24;

    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ProcessDueRecurringTransactionsCommandHandler(ApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<ProcessDueRecurringTransactionsResult> Handle(
        ProcessDueRecurringTransactionsCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var errors = new List<string>();
        var generated = 0;

        var due = await _context.RecurringTransactions
            .Where(x => x.IsActive && x.NextRunAt <= utcNow)
            .OrderBy(x => x.NextRunAt)
            .ToListAsync(cancellationToken);

        foreach (var schedule in due)
        {
            var catchUp = 0;

            while (schedule.IsActive && schedule.NextRunAt <= utcNow && catchUp < MaxCatchUpPerSchedule)
            {
                var command = new CreateTransactionCommand
                {
                    UserId = schedule.ApplicationUserId,
                    WalletId = schedule.WalletId,
                    Amount = schedule.Amount,
                    TransactionType = schedule.TransactionType,
                    CategoryId = schedule.CategoryId,
                    Description = string.IsNullOrWhiteSpace(schedule.Description)
                        ? "Recurring"
                        : schedule.Description,
                    TransactionDate = schedule.NextRunAt
                };

                var result = await _mediator.Send(command, cancellationToken);

                if (!result.Success)
                {
                    errors.Add(
                        $"Schedule {schedule.Id}: {string.Join("; ", result.Errors)}");
                    break;
                }

                generated++;
                schedule.NextRunAt = RecurringScheduleCalculator.GetNextOccurrenceUtc(
                    schedule.NextRunAt,
                    schedule.Frequency);
                schedule.UpdatedAt = DateTime.UtcNow;
                catchUp++;
            }
        }

        if (due.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new ProcessDueRecurringTransactionsResult
        {
            GeneratedCount = generated,
            Errors = errors
        };
    }
}
