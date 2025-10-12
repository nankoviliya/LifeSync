import styles from './ErrorDebugDetails.module.scss';

interface ErrorDebugDetailsProps {
  errorDetails: string;
}

export const ErrorDebugDetails = ({ errorDetails }: ErrorDebugDetailsProps) => (
  <div className={styles['error-details']}>
    <span className={styles['error-details__label']}>Debug info</span>
    <pre className={styles['error-details__content']}>{errorDetails}</pre>
  </div>
);
