import { Button } from '@/components/buttons/Button';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { NewExpenseTransaction } from '@/features/finances/transactions/components/NewExpenseTransaction';
import { NewIncomeTransaction } from '@/features/finances/transactions/components/NewIncomeTransaction';
import { useNewTransactionButtons } from '@/features/finances/transactions/hooks/useNewTransactionButtons';
import { useAppTranslations } from '@/hooks/useAppTranslations';

export const NewTransactionButtons = () => {
  const { translate } = useAppTranslations();

  const {
    isExpenseFormVisible,
    setIsExpenseFormVisible,
    isIncomeFormVisible,
    setIsIncomeFormVisible,
  } = useNewTransactionButtons();

  const handleCloseNewExpenseTransactionModal = () =>
    setIsExpenseFormVisible(false);

  const handleCloseNewIncomeTransactionModal = () =>
    setIsIncomeFormVisible(false);

  return (
    <div className="my-4">
      <div className="flex flex-wrap gap-2">
        <Button
          label={translate('add-new-income-transaction-button-label')}
          onClick={() => setIsIncomeFormVisible(true)}
          outlined
          severity="success"
        />
        <Button
          label={translate('add-new-expense-transaction-button-label')}
          onClick={() => setIsExpenseFormVisible(true)}
          outlined
          severity="danger"
        />
      </div>

      <Dialog
        open={isExpenseFormVisible}
        onOpenChange={setIsExpenseFormVisible}
      >
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>New expense transaction</DialogTitle>
          </DialogHeader>
          <NewExpenseTransaction
            closeForm={handleCloseNewExpenseTransactionModal}
          />
        </DialogContent>
      </Dialog>

      <Dialog open={isIncomeFormVisible} onOpenChange={setIsIncomeFormVisible}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>New income transaction</DialogTitle>
          </DialogHeader>
          <NewIncomeTransaction
            closeForm={handleCloseNewIncomeTransactionModal}
          />
        </DialogContent>
      </Dialog>
    </div>
  );
};
