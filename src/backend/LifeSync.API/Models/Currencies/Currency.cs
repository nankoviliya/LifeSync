using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Models.Currencies;

public class Currency : Entity
{
    // ISO 4217 three-letter code (should be unique)
    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string NativeName { get; set; } = default!;

    public string Symbol { get; set; } = default!;

    // ISO numeric code (e.g. 840 for USD)
    public int NumericCode { get; set; }
}
