using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Features.Categories.Commands;
using Qash.API.Features.Categories.DTOs;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Categories.Handlers;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _context.Categories
            .AnyAsync(x =>
                x.ApplicationUserId == request.UserId &&
                x.Name.ToLower() == name.ToLower() &&
                x.Type == request.Type,
                cancellationToken);

        if (exists)
        {
            return ApiResponse<CategoryDto>.FailResponse(
                "Create category failed.",
                ["Category already exists."]);
        }

        var category = new Category
        {
            Name = name,
            Type = request.Type,
            Icon = request.Icon?.Trim(),
            Color = request.Color?.Trim(),
            ApplicationUserId = request.UserId
        };

        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<CategoryDto>.SuccessResponse(
            _mapper.Map<CategoryDto>(category),
            "Category created successfully.");
    }
}