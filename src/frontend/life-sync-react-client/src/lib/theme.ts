import {
  DEFAULT_THEME,
  EffectiveTheme,
  Theme,
  THEME_STORAGE_KEY,
} from '@/types/theme';

/**
 * Gets the stored theme preference from localStorage
 */
export const getStoredTheme = (): Theme => {
  const stored = localStorage.getItem(THEME_STORAGE_KEY);
  if (stored === 'light' || stored === 'dark' || stored === 'system') {
    return stored;
  }
  return DEFAULT_THEME;
};

/**
 * Gets the system's preferred color scheme
 */
export const getSystemTheme = (): EffectiveTheme => {
  return window.matchMedia('(prefers-color-scheme: dark)').matches
    ? 'dark'
    : 'light';
};

/**
 * Calculates the effective theme based on preference and system setting
 */
export const getEffectiveTheme = (theme: Theme): EffectiveTheme => {
  return theme === 'system' ? getSystemTheme() : theme;
};

/**
 * Initializes theme on app startup (before React renders)
 * Sets data-theme attribute on document root to prevent FOUC
 */
export const initializeTheme = (): void => {
  const theme = getStoredTheme();
  const effectiveTheme = getEffectiveTheme(theme);
  document.documentElement.setAttribute('data-theme', effectiveTheme);
};
