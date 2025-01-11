using LifeSync.API.Models;
using LifeSync.API.Shared;

namespace LifeSync.API.Features.IncomeTracking.Models;

public record AddIncomeDto
{
    public decimal Amount { get; init; } = default!;
    
    public string Currency { get; init; } = default!;
    
    public DateTime Date { get; init; } 

    public string Description { get; init; } = default!; 
}