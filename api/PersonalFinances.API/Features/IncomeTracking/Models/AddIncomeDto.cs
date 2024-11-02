using PersonalFinances.API.Models;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.IncomeTracking.Models;

public record AddIncomeDto
{
    public Money Amount { get; init; } = default!;
    
    public DateTime Date { get; init; } 

    public string Description { get; init; } = default!; 
}