import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useTheme } from '@/hooks/useTheme';
import { Theme } from '@/lib/theme';

export const useThemeToggle = () => {
  const { translate } = useAppTranslations();
  const { theme, effectiveTheme, setTheme } = useTheme();

  const themeOptions: { value: Theme; label: string }[] = [
    { value: 'light', label: translate('theme-light-label') },
    { value: 'dark', label: translate('theme-dark-label') },
    { value: 'system', label: translate('theme-system-label') },
  ];

  return {
    theme,
    effectiveTheme,
    setTheme,
    themeOptions,
  };
};
