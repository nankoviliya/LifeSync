import { useIncomeTracking } from '../hooks/useIncomeTracking';
import styles from './IncomeTracking.module.scss';

export const IncomeTracking = () => {
  const { data, isLoading, isSuccess } = useIncomeTracking();

  return (
    <div className={styles['income-tracking']}>
      {isLoading && <p>Loading...</p>}
      {!isLoading && !data && <p>No records</p>}
      {isSuccess &&
        data &&
        data.map((i) => {
          return <div>{i.description}</div>;
        })}
    </div>
  );
};
