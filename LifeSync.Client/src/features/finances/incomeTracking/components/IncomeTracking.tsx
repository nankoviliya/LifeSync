import { Button } from 'primereact/button';
import { useIncomeTracking } from '../hooks/useIncomeTracking';
import styles from './IncomeTracking.module.scss';
import { Dialog } from 'primereact/dialog';
import { NewIncomeTransaction } from '@/features/finances/incomeTracking/addTransaction/components/NewIncomeTransaction';
import { Card } from 'primereact/card';

export const IncomeTracking = () => {
  const {
    data,
    control,
    isLoading,
    isSuccess,
    isModalVisible,
    setIsModalVisible,
    handleSubmit,
  } = useIncomeTracking();

  return (
    <div className={styles['income-tracking']}>
      <Button
        label="Add new income transaction"
        onClick={() => {
          setIsModalVisible(true);
        }}
      />
      <Dialog
        header="New income transaction"
        visible={isModalVisible}
        style={{ width: '50vw' }}
        onHide={() => {
          if (!isModalVisible) return;
          setIsModalVisible(false);
        }}
      >
        <NewIncomeTransaction control={control} handleSubmit={handleSubmit} />
      </Dialog>
      <div className={styles['income-tracking__list']}>
        {isLoading && <p>Loading...</p>}
        {!isLoading && !data && <p>No records</p>}
        {isSuccess &&
          data &&
          data.map((i) => {
            return (
              <Card
                key={`income-transaction-${i.id}`}
                title={`${i.description} - ${i.date}`}
              >
                <p className="m-0">
                  Transaction amount - {i.amount + ' ' + i.currency}
                </p>
                <p className="m-0">Description - {i.description}</p>
              </Card>
            );
          })}
      </div>
    </div>
  );
};
