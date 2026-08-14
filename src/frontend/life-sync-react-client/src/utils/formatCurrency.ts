export interface FormatCurrencyOptions {
  /** When false, hides minor units (whole-number rounding) for large headline figures. Defaults to true. */
  cents?: boolean;
}

/**
 * Formats a numeric amount as currency via the Intl API. Setting `cents: false`
 * hides minor units (whole-number rounding) for large headline figures. Falls back
 * to a grouped number + raw code if the currency code is invalid.
 *
 * @param amount - The numeric value to format
 * @param currency - ISO 4217 currency code (e.g., 'USD', 'EUR')
 * @param options - Optional configuration; `cents` defaults to true
 * @returns Formatted currency string
 */
export const formatCurrency = (
  amount: number,
  currency: string,
  options: FormatCurrencyOptions = {},
): string => {
  const { cents = true } = options;

  try {
    return new Intl.NumberFormat(undefined, {
      style: 'currency',
      currency,
      minimumFractionDigits: cents ? 2 : 0,
      maximumFractionDigits: cents ? 2 : 0,
    }).format(amount);
  } catch {
    // Unknown/invalid currency code — degrade gracefully to a grouped number + raw code.
    const number = new Intl.NumberFormat(undefined, {
      minimumFractionDigits: cents ? 2 : 0,
      maximumFractionDigits: cents ? 2 : 0,
    }).format(amount);
    return `${number} ${currency}`.trim();
  }
};

/**
 * Formats a signed amount — positive values get a leading '+', negative values
 * a leading '-'. The magnitude is formatted by formatCurrency. Useful for
 * displaying transaction deltas or account changes.
 *
 * @param amount - The numeric value to format (sign is derived from this)
 * @param currency - ISO 4217 currency code (e.g., 'USD', 'EUR')
 * @param options - Optional configuration passed to formatCurrency
 * @returns Formatted signed currency string
 */
export const formatSignedCurrency = (
  amount: number,
  currency: string,
  options: FormatCurrencyOptions = {},
): string => {
  const sign = amount > 0 ? '+ ' : amount < 0 ? '- ' : '';
  return `${sign}${formatCurrency(Math.abs(amount), currency, options)}`;
};
