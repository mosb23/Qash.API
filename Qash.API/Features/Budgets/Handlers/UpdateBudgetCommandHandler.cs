using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Enums;
using Qash.API.Features.Budgets.Commands;
using Qash.API.Features.Budgets.DTOs;
using Qash.API.Infrastructure.Data;

namespace Qash.API.Features.Budgets.Handlers;

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, ApiResponse<BudgetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateBudgetCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BudgetDto>> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(x => x.Category)
            .FirstOrDefaultAsync(
                x => x.Id == request.BudgetId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (budget is null)
        {
            return ApiResponse<BudgetDto>.FailResponse(
                "Update budget failed.",
                ["Budget was not found."]);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == request.CategoryId && x.ApplicationUserId == request.UserId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<BudgetDto>.FailResponse(
                "Update budget failed.",
                ["Category was not found."]);
        }

        if (category.Type != CategoryType.Expense)
        {
            return ApiResponse<BudgetDto>.FailResponse(
                "Update budget failed.",
                ["Budgets can only be linked to expense categories."]);
        }

        var duplicate = await _context.Budgets
            .AnyAsync(
                x =>
                    x.ApplicationUserId == request.UserId &&
                    x.Id != budget.Id &&
                    x.CategoryId == request.CategoryId &&
                    x.Year == request.Year &&
                    x.Month == request.Month,
                cancellationToken);

        if (duplicate)
        {
            return ApiResponse<BudgetDto>.FailResponse(
                "Update budget failed.",
                ["A budget for this category and month already exists."]);
        }

        budget.CategoryId = category.Id;
        budget.Year = request.Year;
        budget.Month = request.Month;
        budget.Amount = request.Amount;
        budget.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(budget).Reference(x => x.Category).LoadAsync(cancellationToken);

        return ApiResponse<BudgetDto>.SuccessResponse(
            _mapper.Map<BudgetDto>(budget),
            "Budget updated successfully.");
    }
}
