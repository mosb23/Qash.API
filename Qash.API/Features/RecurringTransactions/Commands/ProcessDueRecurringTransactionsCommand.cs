using MediatR;

namespace Qash.API.Features.RecurringTransactions.Commands;

public class ProcessDueRecurringTransactionsCommand : IRequest<ProcessDueRecurringTransactionsResult>
{
}

public class ProcessDueRecurringTransactionsResult
{
    public int GeneratedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
