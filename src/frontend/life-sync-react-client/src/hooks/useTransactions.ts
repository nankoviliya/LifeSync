import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { ExpenseType, TransactionType } from '@/types/transactionTypes';

export interface ITransactionsFiltersModel {
  description: string | null;
  startDate: Date | null;
  endDate: Date | null;
  expenseTypes: ExpenseType[];
  transactionTypes: TransactionType[];
}

export interface ITransactionGetModel {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description: string;
  transactionType: TransactionType;
}

export interface IExpenseTransactionGetModel extends ITransactionGetModel {
  expenseType: ExpenseType;
}

export type IIncomeTransactionGetModel = ITransactionGetModel;

export interface IExpenseSummaryData {
  totalSpent: number;
  totalSpentOnNeeds: number;
  totalSpentOnWants: number;
  totalSpentOnSavings: number;
  currency: string;
}

export interface IIncomeSummaryData {
  totalIncome: number;
  currency: string;
}

export interface ITransactionsGetModel {
  transactions: IExpenseTransactionGetModel[] | IIncomeTransactionGetModel[];
  expenseSummary: IExpenseSummaryData;
  incomeSummary: IIncomeSummaryData;
  transactionsCount: number;
}

export const useTransactions = (filters: ITransactionsFiltersModel) => {
  const { data, isLoading, isSuccess, isError, error, refetch } =
    useReadQuery<ITransactionsGetModel>({
      endpoint: endpointsOptions.getUserTransactions.endpoint,
      queryKey: [endpointsOptions.getUserTransactions.key],
      config: { params: filters },
      staleTime: 86_400_000,
    });

  return {
    data,
    isLoading,
    isSuccess,
    isError,
    error,
    refetch,
  };
};
