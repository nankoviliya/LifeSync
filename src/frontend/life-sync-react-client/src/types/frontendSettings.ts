export interface IFrontendSettings {
  languageOptions: ILanguageOption[];
  currencyOptions: ICurrencyOption[];
}

export interface ILanguageOption {
  id: string;
  name: string;
  code: string;
}

export interface ICurrencyOption {
  id: string;
  code: string;
  name: string;
  nativeName: string;
  symbol: string;
}
