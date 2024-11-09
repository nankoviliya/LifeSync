import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link } from 'react-router-dom';
import styles from './Home.module.scss';

export const Home = () => {
  return (
    <div className={styles["home"]}>
      <Link to={routePaths.incomeTracking.path}>
        {routePaths.incomeTracking.name}
      </Link>
      <Link to={routePaths.expenseTracking.path}>
        {routePaths.expenseTracking.name}
      </Link>
    </div>
  );
};
