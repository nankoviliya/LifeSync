import { Button } from '@/components/buttons/Button';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const MainErrorFallback = () => {
  const { translate } = useAppTranslations();

  return (
    <div
      className="flex h-screen w-screen flex-col items-center justify-center gap-8 text-destructive"
      role="alert"
    >
      <span className="text-xl font-semibold">
        {translate('main-error-fallback-label')}
      </span>
      <Button
        className="mt-4"
        onClick={() => window.location.assign(window.location.origin)}
      >
        {translate('main-error-fallback-refresh-button-label')}
      </Button>
    </div>
  );
};
