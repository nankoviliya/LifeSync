import { useReadQuery } from '@infrastructure/api/hooks/useReadQuery';
import styles from './App.module.scss';
import { endpointsOptions } from '@infrastructure/api/endpoints/endpointsOptions';
import { Link } from 'react-router-dom';
import { routes } from '@infrastructure/routing/routes';
import { routePaths } from '@infrastructure/routing/routePaths';

function AppTest() {
  const { data, isLoading, isSuccess } = useReadQuery<string[]>({
    endpoint: endpointsOptions.getBaseInfo.endpoint,
    queryKey: [endpointsOptions.getBaseInfo.key],
    staleTime: 86_400_000,
  });

  return (
    <div className={styles['App']}>
      <Link to={routePaths.incomeTracking.path}>
        {routePaths.incomeTracking.name}
      </Link>
      <Link to={routePaths.expenseTracking.path}>
        {routePaths.expenseTracking.name}
      </Link>
    </div>
  );
}

export default AppTest;
