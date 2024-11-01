import { useReadQuery } from '@infrastructure/api/hooks/useReadQuery';
import styles from './App.module.scss';
import { endpointsOptions } from '@infrastructure/api/endpoints/endpointsOptions';

function AppTest() {
  const { data, isLoading, isSuccess } = useReadQuery<string[]>({
    endpoint: endpointsOptions.getBaseInfo.endpoint,
    queryKey: [endpointsOptions.getBaseInfo.key],
    staleTime: 86_400_000,
  });

  return (
    <div className={styles['App']}>
      {isLoading && <p>Loading...</p>}
      {isSuccess && <p>{data.join(', ')}</p>}
    </div>
  );
}

export default AppTest;
