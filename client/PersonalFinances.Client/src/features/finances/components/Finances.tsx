import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link } from 'react-router-dom';
import styles from './Finances.module.scss';

export const Finances = () => {
  return (
    <div className={styles['finances']}>
      <Link to={routePaths.incomeTracking.path}>
        {routePaths.incomeTracking.name}
      </Link>
      <Link to={routePaths.expenseTracking.path}>
        {routePaths.expenseTracking.name}
      </Link>
    </div>
  );
};
