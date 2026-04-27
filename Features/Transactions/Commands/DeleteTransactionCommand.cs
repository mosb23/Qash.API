using MediatR;
using Qash.API.Common.Responses;
using System;

namespace Qash.API.Features.Transactions.Commands;

public class DeleteTransactionCommand : IRequest<ApiResponse<string>>
{
    public Guid UserId { get; set; }

    public Guid TransactionId { get; set; }
}