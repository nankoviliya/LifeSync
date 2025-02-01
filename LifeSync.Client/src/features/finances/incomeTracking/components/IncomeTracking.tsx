import { Button } from 'primereact/button';
import { useIncomeTracking } from '../hooks/useIncomeTracking';
import styles from './IncomeTracking.module.scss';
import { Dialog } from 'primereact/dialog';
import { NewIncomeTransaction } from '@/features/finances/incomeTracking/addTransaction/components/NewIncomeTransaction';
import { Card } from 'primereact/card';
import { useAppTranslations } from '@/infrastructure/translations/hooks/useAppTranslations';

export const IncomeTracking = () => {
  const { translate } = useAppTranslations();

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
        label={translate('add-new-income-transaction-button-label')}
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
                  {translate('transaction-amount-label')} -{' '}
                  {i.amount + ' ' + i.currency}
                </p>
                <p className="m-0">
                  {translate('transaction-description-label')} - {i.description}
                </p>
              </Card>
            );
          })}
      </div>
    </div>
  );
};
