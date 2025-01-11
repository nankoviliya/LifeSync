import { Card } from 'primereact/card';
import { useExpenseTracking } from '../hooks/useExpenseTracking';
import styles from './ExpenseTracking.module.scss';
import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';
import { NewExpenseTransaction } from '@/features/finances/expenseTracking/addTransaction/components/NewExpenseTransaction';

export const ExpenseTracking = () => {
  const {
    data,
    control,
    isLoading,
    isSuccess,
    isModalVisible,
    setIsModalVisible,
    handleSubmit,
  } = useExpenseTracking();

  return (
    <div className={styles['expense-tracking']}>
      <Button
        label="Add new expense"
        onClick={() => {
          setIsModalVisible(true);
        }}
      />
      <Dialog
        header="New transaction"
        visible={isModalVisible}
        style={{ width: '50vw' }}
        onHide={() => {
          if (!isModalVisible) return;
          setIsModalVisible(false);
        }}
      >
        <NewExpenseTransaction control={control} handleSubmit={handleSubmit} />
      </Dialog>
      <div className={styles['expense-tracking__list']}>
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
    </div>
  );
};
