import { useForm } from 'react-hook-form';

import { ITransactionsFiltersModel } from '@/features/finances/transactions/models/transactionsFiltersModel';
import {
  ExpenseType,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';

export const useTransactionsFilters = () => {
  const formDefaultValues = {
    description: '',
    startDate: null,
    endDate: null,
    expenseTypes: [ExpenseType.Needs, ExpenseType.Wants, ExpenseType.Savings],
    transactionTypes: [TransactionType.Expense, TransactionType.Income],
  };

  const { control, watch, reset } = useForm<ITransactionsFiltersModel>({
    defaultValues: formDefaultValues,
  });

  const handleReset = async (): Promise<void> => {
    reset(formDefaultValues);
  };

  return {
    control,
    watch,
    handleReset,
  };
};
