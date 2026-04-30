using MediatR;
using Qash.API.Common.Responses;
using System;

namespace Qash.API.Features.Categories.Commands;

public class DeleteCategoryCommand : IRequest<ApiResponse<string>>
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DeleteCategoryCommand(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }
}