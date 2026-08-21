import { useTransactions } from '@/hooks/useTransactions';
import { useAuth } from '@/stores/AuthProvider';
import { ExpenseType, TransactionType } from '@/types/transactionTypes';
import {
  getCurrentMonthFirstDayDate,
  getCurrentMonthLastDayDate,
} from '@/utils/dateUtilities';

export const useDashboardData = () => {
  const { user } = useAuth();

  // Default to the current month, all types — same defaults the Money filters use.
  const { data, isLoading, isSuccess, isError, refetch } = useTransactions({
    description: null,
    startDate: getCurrentMonthFirstDayDate(),
    endDate: getCurrentMonthLastDayDate(),
    expenseTypes: [ExpenseType.Needs, ExpenseType.Wants, ExpenseType.Savings],
    transactionTypes: [TransactionType.Expense, TransactionType.Income],
  });

  return { user, data, isLoading, isSuccess, isError, refetch };
};
