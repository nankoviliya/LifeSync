import { Button } from 'primereact/button';

import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './MainErrorFallback.module.scss';

export const MainErrorFallback = () => {
  const { translate } = useAppTranslations();

  return (
    <div className={styles['main-error-fallback']} role="alert">
      <span className={styles['main-error-fallback__header']}>
        {translate('main-error-fallback-label')}
      </span>
      <Button
        className={styles['main-error-fallback__button']}
        onClick={() => window.location.assign(window.location.origin)}
      >
        {translate('main-error-fallback-refresh-button-label')}
      </Button>
    </div>
  );
};
