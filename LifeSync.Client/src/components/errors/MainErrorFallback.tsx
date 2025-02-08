import { Button } from 'primereact/button';
import styles from './MainErrorFallback.module.scss';
import { useAppTranslations } from '@/infrastructure/translations/hooks/useAppTranslations';

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
