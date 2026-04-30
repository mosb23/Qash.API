using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Categories.DTOs;
using Qash.API.Features.Categories.Queries;
using Qash.API.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Categories.Handlers;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == request.UserId);

        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type.Value);
        }

        var categories = await query
            .OrderBy(x => x.Type)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<CategoryDto>>.SuccessResponse(
            _mapper.Map<List<CategoryDto>>(categories),
            "Categories retrieved successfully.");
    }
}