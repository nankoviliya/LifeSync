import { Button } from 'primereact/button';
import { Dialog } from 'primereact/dialog';

import { NewExpenseTransaction } from '@/features/finances/transactions/components/NewExpenseTransaction';
import { NewIncomeTransaction } from '@/features/finances/transactions/components/NewIncomeTransaction';
import { useNewExpenseTransaction } from '@/features/finances/transactions/hooks/useNewExpenseTransaction';
import { useNewIncomeTransaction } from '@/features/finances/transactions/hooks/useNewIncomeTransaction';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './NewTransactionButtons.module.scss';

export const NewTransactionButtons = () => {
  const { translate } = useAppTranslations();

  const {
    control: newExpenseTransactionControl,
    handleSubmit: newExpenseTransactionHandleSubmit,
    isModalVisible: newExpenseTransactionIsModalVisible,
    setIsModalVisible: newExpenseTransactionSetIsModalVisible,
  } = useNewExpenseTransaction();

  const {
    control: newIncomeTransactionControl,
    handleSubmit: newIncomeTransactionHandleSubmit,
    isModalVisible: newIncomeTransactionIsModalVisible,
    setIsModalVisible: newIncomeTransactionSetIsModalVisible,
  } = useNewIncomeTransaction();

  const handleOpenNewExpenseTransactionModal = () =>
    newExpenseTransactionSetIsModalVisible(true);
  const handleCloseNewExpenseTransactionModal = () =>
    newExpenseTransactionSetIsModalVisible(false);

  const handleOpenNewIncomeTransactionModal = () =>
    newIncomeTransactionSetIsModalVisible(true);
  const handleCloseNewIncomeTransactionModal = () =>
    newIncomeTransactionSetIsModalVisible(false);

  return (
    <div className={styles['new-transaction-buttons']}>
      <div className={styles['new-transaction-buttons__container']}>
        <Button
          label={translate('add-new-income-transaction-button-label')}
          onClick={handleOpenNewIncomeTransactionModal}
          outlined
          severity="success"
        />
        <Button
          label={translate('add-new-expense-transaction-button-label')}
          onClick={handleOpenNewExpenseTransactionModal}
          outlined
          severity="danger"
        />
      </div>

      <Dialog
        header="New expense transaction"
        visible={newExpenseTransactionIsModalVisible}
        style={{ width: '50vw' }}
        onHide={handleCloseNewExpenseTransactionModal}
      >
        <NewExpenseTransaction
          control={newExpenseTransactionControl}
          handleSubmit={newExpenseTransactionHandleSubmit}
        />
      </Dialog>

      <Dialog
        header="New income transaction"
        visible={newIncomeTransactionIsModalVisible}
        style={{ width: '50vw' }}
        onHide={handleCloseNewIncomeTransactionModal}
      >
        <NewIncomeTransaction
          control={newIncomeTransactionControl}
          handleSubmit={newIncomeTransactionHandleSubmit}
        />
      </Dialog>
    </div>
  );
};
