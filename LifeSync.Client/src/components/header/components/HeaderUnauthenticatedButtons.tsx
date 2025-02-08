import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link } from 'react-router-dom';
import styles from './HeaderUnauthenticatedButtons.module.scss';
import { classNames } from 'primereact/utils';

export const HeaderUnauthenticatedButtons = () => {
  return (
    <div className={styles['header-unauthenticated-buttons']}>
      <Link
        className={classNames(
          styles['header-unauthenticated-buttons__button'],
          styles['header-unauthenticated-buttons__button__login'],
        )}
        to={routePaths.login.path}
      >
        {routePaths.login.name}
      </Link>
      <Link
        className={classNames(
          styles['header-unauthenticated-buttons__button'],
          styles['header-unauthenticated-buttons__button--signup'],
        )}
        to={routePaths.register.path}
      >
        {routePaths.register.name}
      </Link>
    </div>
  );
};
