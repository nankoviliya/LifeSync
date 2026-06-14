import { useAppTranslations } from '@/hooks/useAppTranslations';
import { useTheme } from '@/hooks/useTheme';
import { cn } from '@/lib/utils';

export const ThemeSegmentedToggle = () => {
  const { effectiveTheme, setTheme } = useTheme();
  const { translate } = useAppTranslations();
  const lightLabel = translate('theme-light-label', { defaultValue: 'Light' });
  const darkLabel = translate('theme-dark-label', { defaultValue: 'Dark' });

  const isDark = effectiveTheme === 'dark';

  return (
    <div
      role="group"
      aria-label="Theme"
      className="inline-flex items-center gap-0.5 rounded-full border border-border bg-muted p-0.5 text-[12px]"
    >
      <button
        type="button"
        onClick={() => setTheme('light')}
        aria-pressed={!isDark}
        className={cn(
          'rounded-full px-3 py-1 transition-colors',
          !isDark
            ? 'bg-card text-foreground shadow-xs'
            : 'text-muted-foreground hover:text-foreground',
        )}
      >
        {lightLabel}
      </button>
      <button
        type="button"
        onClick={() => setTheme('dark')}
        aria-pressed={isDark}
        className={cn(
          'rounded-full px-3 py-1 transition-colors',
          isDark
            ? 'bg-card text-foreground shadow-xs'
            : 'text-muted-foreground hover:text-foreground',
        )}
      >
        {darkLabel}
      </button>
    </div>
  );
};
