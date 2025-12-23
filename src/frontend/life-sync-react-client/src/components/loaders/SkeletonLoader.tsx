import styles from './SkeletonLoader.module.scss';

export const SkeletonLoader = () => {
  return (
    <div className={styles['skeleton-loader']} aria-label="Loading content">
      <div className={styles['skeleton-loader__header']}>
        <div className={styles['skeleton-loader__header-logo']}>
          <div className={styles['skeleton-loader__header-logo-circle']} />
          <div className={styles['skeleton-loader__header-logo-text']} />
        </div>
        <div className={styles['skeleton-loader__header-actions']}>
          <div className={styles['skeleton-loader__header-action']} />
          <div className={styles['skeleton-loader__header-action']} />
        </div>
      </div>

      <div className={styles['skeleton-loader__content']}>
        <div className={styles['skeleton-loader__content-card']} />
      </div>

      <div className={styles['skeleton-loader__footer']}>
        <div
          className={`${styles['skeleton-loader__footer-bar']} ${styles['skeleton-loader__footer-bar--short']}`}
        />
        <div
          className={`${styles['skeleton-loader__footer-bar']} ${styles['skeleton-loader__footer-bar--long']}`}
        />
      </div>
    </div>
  );
};
