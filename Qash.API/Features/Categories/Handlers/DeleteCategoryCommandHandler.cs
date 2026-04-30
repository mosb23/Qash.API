using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Categories.Commands;
using Qash.API.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Categories.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;

    public DeleteCategoryCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.ApplicationUserId == request.UserId, cancellationToken);

        if (category is null)
        {
            return ApiResponse<string>.FailResponse(
                "Delete category failed.",
                ["Category was not found."]);
        }

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.SuccessResponse(
            "Category deleted",
            "Category deleted successfully.");
    }
}