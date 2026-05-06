using MediatR;
using Qash.API.Common.Responses;

namespace Qash.API.Features.Budgets.Commands;

public class DeleteBudgetCommand : IRequest<ApiResponse<string>>
{
    public Guid UserId { get; set; }

    public Guid BudgetId { get; set; }
}
