﻿namespace LifeSync.API.Features.FrontendSettings.Models;

public class FrontendSettingsResponse
{
    public List<LanguageOption> LanguageOptions { get; set; } = [];

    public List<CurrencyOption> CurrencyOptions { get; set; } = [];
}
