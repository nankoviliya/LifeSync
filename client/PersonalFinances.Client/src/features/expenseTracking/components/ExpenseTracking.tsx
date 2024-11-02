import { Card } from 'primereact/card';
import { useExpenseTracking } from '../hooks/useExpenseTracking';
import styles from './ExpenseTracking.module.scss';

export const ExpenseTracking = () => {
  const { data, isLoading, isSuccess } = useExpenseTracking();

  return (
    <div className={styles['expense-tracking']}>
      {isLoading && <p>Loading...</p>}
      {!isLoading && !data && <p>No records</p>}
      {isSuccess &&
        data &&
        data.map((i) => {
          return (
            <Card
              key={`expense-transaction-${i.id}`}
              title={`${i.description} - ${i.date}`}
            >
              <p className="m-0">
                Transaction amount - {i.amount + ' ' + i.currency}
              </p>
              <p className="m-0">Description - {i.description}</p>
              <p className="m-0">Expense type - {i.expenseType}</p>
            </Card>
          );
        })}
    </div>
  );
};
