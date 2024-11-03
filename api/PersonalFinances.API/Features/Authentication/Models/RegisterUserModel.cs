using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.Authentication.Models;

public class RegisterUserModel
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string CurrencyPreferenceCode { get; set; } = "USD";
    public Money Balance { get; init; } = default!;
    public Currency CurrencyPreference { get; init; } = default!;
}