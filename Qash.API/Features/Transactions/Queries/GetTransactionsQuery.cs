using MediatR;
using Qash.API.Common.Responses;
using Qash.API.Features.Transactions.DTOs;
using System;
using System.Collections.Generic;

namespace Qash.API.Features.Transactions.Queries;

public class GetTransactionsQuery : IRequest<ApiResponse<List<TransactionDto>>>
{
    public Guid UserId { get; set; }

    public GetTransactionsQuery(Guid userId)
    {
        UserId = userId;
    }
}