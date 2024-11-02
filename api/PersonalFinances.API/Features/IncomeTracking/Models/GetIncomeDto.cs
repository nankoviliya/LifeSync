using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.IncomeTracking.Models;

public class GetIncomeDto
{
    public Guid Id { get; init; } // Primary Key

    public Money Amount { get; init; } = default!;// Income amount
    
    public DateTime Date { get; init; } // Date of income

    public string Description { get; init; } = default!; // Optional details
}