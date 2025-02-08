import { Button } from 'primereact/button';
import { Card } from 'primereact/card';
import { Dialog } from 'primereact/dialog';

import { NewExpenseTransaction } from '@/features/finances/expenseTracking/addTransaction/components/NewExpenseTransaction';
import { useExpenseTracking } from '@/features/finances/expenseTracking/hooks/useExpenseTracking';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './ExpenseTracking.module.scss';

export const ExpenseTracking = () => {
  const { translate } = useAppTranslations();

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
        label={translate('add-new-expense-transaction-button-label')}
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
          data.expenseTransactions.map((i) => {
            return (
              <Card
                key={`expense-transaction-${i.id}`}
                title={`${i.description} - ${i.date}`}
              >
                <p className="m-0">
                  {translate('transaction-amount-label')} -{' '}
                  {i.amount + ' ' + i.currency}
                </p>
                <p className="m-0">
                  {translate('transaction-description-label')} - {i.description}
                </p>
                <p className="m-0">
                  {translate('expense-transaction-type-label')} -{' '}
                  {i.expenseType}
                </p>
              </Card>
            );
          })}
      </div>
    </div>
  );
};
