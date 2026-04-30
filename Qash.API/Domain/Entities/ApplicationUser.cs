using Qash.API.Domain.Common;
using System.Collections.Generic;

namespace Qash.API.Domain.Entities;

public class ApplicationUser : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsPhoneNumberVerified { get; set; } = false;

    public string PasswordHash { get; set; } = string.Empty;

    public List<RefreshToken> RefreshTokens { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";

}