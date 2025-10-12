import type { FallbackProps } from 'react-error-boundary';

import { Button } from '@/components/buttons/Button';
import { ErrorDebugDetails } from '@/components/errors/debugDetails/ErrorDebugDetails';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './GlobalErrorFallback.module.scss';

type GlobalErrorFallbackProps = Partial<FallbackProps>;

export const GlobalErrorFallback = ({ error }: GlobalErrorFallbackProps) => {
  const { translate } = useAppTranslations();

  const showErrorDetails = import.meta.env.DEV && !!error;
  const errorDetails = error?.stack ?? error?.message ?? 'Unknown error';

  return (
    <div className={styles['error-fallback']} role="alert">
      <span className={styles['error-fallback__header']}>
        {translate('main-error-fallback-label')}
      </span>

      {showErrorDetails && <ErrorDebugDetails errorDetails={errorDetails} />}

      <Button
        className={styles['error-fallback__button']}
        onClick={() => window.location.assign(window.location.origin)}
      >
        {translate('main-error-fallback-refresh-button-label')}
      </Button>
    </div>
  );
};
