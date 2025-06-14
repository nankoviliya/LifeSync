import { Dialog } from 'primereact/dialog';

import { Button } from '@/components/buttons/Button';
import { NewExpenseTransaction } from '@/features/finances/transactions/components/NewExpenseTransaction';
import { NewIncomeTransaction } from '@/features/finances/transactions/components/NewIncomeTransaction';
import { useNewTransactionButtons } from '@/features/finances/transactions/hooks/useNewTransactionButtons';
import { useAppTranslations } from '@/hooks/useAppTranslations';

import styles from './NewTransactionButtons.module.scss';

export const NewTransactionButtons = () => {
  const { translate } = useAppTranslations();

  const {
    isExpenseFormVisible,
    setIsExpenseFormVisible,
    isIncomeFormVisible,
    setIsIncomeFormVisible,
  } = useNewTransactionButtons();

  const handleOpenNewExpenseTransactionModal = () =>
    setIsExpenseFormVisible(true);
  const handleCloseNewExpenseTransactionModal = () =>
    setIsExpenseFormVisible(false);

  const handleOpenNewIncomeTransactionModal = () =>
    setIsIncomeFormVisible(true);
  const handleCloseNewIncomeTransactionModal = () =>
    setIsIncomeFormVisible(false);

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
        visible={isExpenseFormVisible}
        style={{ width: '50vw' }}
        onHide={handleCloseNewExpenseTransactionModal}
      >
        <NewExpenseTransaction
          closeForm={handleCloseNewExpenseTransactionModal}
        />
      </Dialog>

      <Dialog
        header="New income transaction"
        visible={isIncomeFormVisible}
        style={{ width: '50vw' }}
        onHide={handleCloseNewIncomeTransactionModal}
      >
        <NewIncomeTransaction
          closeForm={handleCloseNewIncomeTransactionModal}
        />
      </Dialog>
    </div>
  );
};
