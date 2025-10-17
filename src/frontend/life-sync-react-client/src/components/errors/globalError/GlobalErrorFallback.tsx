import { useState } from 'react';
import type { FallbackProps } from 'react-error-boundary';

import sadFace from '@/assets/sad-face.svg';
import { Button } from '@/components/buttons/Button';
import { ErrorDebugDetails } from '@/components/errors/debugDetails/ErrorDebugDetails';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './GlobalErrorFallback.module.scss';

type GlobalErrorFallbackProps = Partial<FallbackProps>;

export const GlobalErrorFallback = ({
  error,
  resetErrorBoundary,
}: GlobalErrorFallbackProps) => {
  const { translate } = useAppTranslations();

  const [retryCount, setRetryCount] = useState<number>(0);
  const MAX_RETRIES = 3;

  const showErrorDetails = import.meta.env.DEV && !!error;
  const errorDetails = error?.stack ?? error?.message ?? 'Unknown error';

  const title = translate('global-error-fallback-label', {
    defaultValue: '...',
  });

  const description = translate('global-error-fallback-description', {
    defaultValue: '...',
  });

  const refreshLabel = translate('global-error-fallback-refresh-button-label', {
    defaultValue: '...',
  });

  return (
    <div className={styles['error-fallback']}>
      <div className={styles['error-fallback__card']} role="alert">
        <div className={styles['error-fallback__icon']} aria-hidden="true">
          <img
            src={sadFace}
            className={styles['error-fallback__icon-img']}
            alt=""
            role="presentation"
          />
        </div>
        <h1 className={styles['error-fallback__title']}>{title}</h1>
        <p className={styles['error-fallback__description']}>{description}</p>
        {showErrorDetails && (
          <div className={styles['error-fallback__debug']}>
            <ErrorDebugDetails errorDetails={errorDetails} />
          </div>
        )}
        <Button
          className={styles['error-fallback__button']}
          onClick={() => {
            if (retryCount >= MAX_RETRIES) {
              window.location.assign(window.location.origin);
            } else {
              setRetryCount((prev) => prev + 1);
              resetErrorBoundary?.();
            }
          }}
        >
          {refreshLabel}
        </Button>
        <div className={styles['error-fallback__brand']} aria-label="LifeSync">
          <span className={styles['error-fallback__brand-name']}>LifeSync</span>
        </div>
      </div>
    </div>
  );
};
