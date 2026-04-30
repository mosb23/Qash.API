using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.Commands;
using Qash.API.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Auth.Handlers;

public class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public VerifyPhoneCommandHandler(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<string>> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        var demoCode = _configuration["DemoOtp:VerificationCode"] ?? "00000";

        if (request.VerificationCode != demoCode)
        {
            return ApiResponse<string>.FailResponse(
                "Phone verification failed.",
                ["Invalid verification code."]);
        }

        var phone = request.PhoneNumber.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phone, cancellationToken);

        if (user is null)
        {
            return ApiResponse<string>.FailResponse(
                "Phone verification failed.",
                ["No account found with this phone number."]);
        }

        if (user.IsPhoneNumberVerified)
        {
            return ApiResponse<string>.SuccessResponse(
                "Phone already verified",
                "Phone number is already verified.");
        }

        user.IsPhoneNumberVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<string>.SuccessResponse(
            "Phone verified",
            "Phone number verified successfully.");
    }
}