import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link, Outlet } from 'react-router-dom';
import styles from './App.module.scss';

export const App = () => {
  return (
    <div className={styles['App']}>
      <Link to={routePaths.login.path}>{routePaths.login.name}</Link>
      <Outlet />
    </div>
  );
};
