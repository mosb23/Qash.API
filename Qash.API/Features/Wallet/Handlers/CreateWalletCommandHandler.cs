using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Wallet.Commands;
using Qash.API.Features.Wallet.DTOs;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

using WalletEntity = Qash.API.Domain.Entities.Wallet;

namespace Qash.API.Features.Wallet.Handlers;

public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, ApiResponse<WalletDto>>
{
  private readonly ApplicationDbContext _context;
  private readonly IMapper _mapper;

  public CreateWalletCommandHandler(ApplicationDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  public async Task<ApiResponse<WalletDto>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
  {
    var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken);

    if (!userExists)
    {
      return ApiResponse<WalletDto>.FailResponse("Create wallet failed.", ["User not found."]);
    }

    var wallet = new WalletEntity
    {
      ApplicationUserId = request.UserId,
      Name = request.Name.Trim(),
      Currency = request.Currency.Trim().ToUpper(),
      Balance = request.InitialBalance
    };

    await _context.Wallets.AddAsync(wallet, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    var dto = _mapper.Map<WalletDto>(wallet);

    return ApiResponse<WalletDto>.SuccessResponse(dto, "Wallet created successfully.");
  }
}