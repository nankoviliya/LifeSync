namespace LifeSync.API.Features.FrontendSettings.Models
{
    public class CurrencyOption
    {
        public Guid Id { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string NativeName { get; set; } = default!;

        public string Symbol { get; set; } = default!;
    }
}
