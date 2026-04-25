using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.Commands;
using Qash.API.Features.Auth.DTOs;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Auth.Handlers;

public class RequestForgotPasswordCodeCommandHandler
    : IRequestHandler<RequestForgotPasswordCodeCommand, ApiResponse<ForgotPasswordCodeResponseDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public RequestForgotPasswordCodeCommandHandler(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<ForgotPasswordCodeResponseDto>> Handle(
        RequestForgotPasswordCodeCommand request,
        CancellationToken cancellationToken)
    {
        var phone = request.PhoneNumber.Trim();

        var userExists = await _context.Users
            .AnyAsync(x => x.PhoneNumber == phone, cancellationToken);

        if (!userExists)
        {
            return ApiResponse<ForgotPasswordCodeResponseDto>.FailResponse(
                "Request failed.",
                ["No account found with this phone number."]);
        }

        var code = _configuration["DemoOtp:VerificationCode"] ?? "00000";

        return ApiResponse<ForgotPasswordCodeResponseDto>.SuccessResponse(
            new ForgotPasswordCodeResponseDto
            {
                PhoneNumber = phone,
                VerificationCode = code
            },
            "Verification code generated successfully.");
    }
}