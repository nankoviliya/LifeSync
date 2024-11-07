import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';
import { routePaths } from '@/infrastructure/routing/routePaths';
import { Link } from 'react-router-dom';
import styles from './App.module.scss';

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
