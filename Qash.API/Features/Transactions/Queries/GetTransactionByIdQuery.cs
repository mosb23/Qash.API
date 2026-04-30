using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Transactions.DTOs;
using System;

namespace Qash.API.Features.Transactions.Queries;

public class GetTransactionByIdQuery : IRequest<ApiResponse<TransactionDto>>
{
    public Guid UserId { get; set; }

    public Guid TransactionId { get; set; }

    public GetTransactionByIdQuery(Guid userId, Guid transactionId)
    {
        UserId = userId;
        TransactionId = transactionId;
    }
}