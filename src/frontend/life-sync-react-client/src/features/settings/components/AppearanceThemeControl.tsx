import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useTheme } from '@/hooks/useTheme';
import { cn } from '@/lib/utils';
import { Theme } from '@/types/theme';

const OPTIONS: ReadonlyArray<{
  value: Theme;
  labelKey: string;
  fallback: string;
}> = [
  { value: 'light', labelKey: 'theme-light-label', fallback: 'Light' },
  { value: 'system', labelKey: 'theme-system-label', fallback: 'System' },
  { value: 'dark', labelKey: 'theme-dark-label', fallback: 'Dark' },
];

export const AppearanceThemeControl = () => {
  const { theme, setTheme } = useTheme();
  const { translate } = useAppTranslations();

  return (
    <div
      role="group"
      aria-label={translate('settings-theme-label', { defaultValue: 'Theme' })}
      className="inline-flex items-center gap-0.5 rounded-full border border-border bg-muted p-0.5 text-[12px]"
    >
      {OPTIONS.map((opt) => {
        const active = theme === opt.value;
        return (
          <button
            key={opt.value}
            type="button"
            onClick={() => setTheme(opt.value)}
            aria-pressed={active}
            className={cn(
              'rounded-full px-3 py-1 transition-colors',
              active
                ? 'bg-card text-foreground shadow-xs'
                : 'text-muted-foreground hover:text-foreground',
            )}
          >
            {translate(opt.labelKey, { defaultValue: opt.fallback })}
          </button>
        );
      })}
    </div>
  );
};
