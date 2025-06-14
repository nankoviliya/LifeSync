import { classNames } from 'primereact/utils';
import { Link } from 'react-router-dom';

import { routePaths } from '@/config/routing/routePaths';

import styles from './HeaderUnauthenticatedButtons.module.scss';

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
