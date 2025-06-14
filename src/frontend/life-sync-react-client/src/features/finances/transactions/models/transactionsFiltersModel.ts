import {
  ExpenseType,
  TransactionType,
} from '@/features/finances/transactions/models/transactionsGetModel';

export interface ITransactionsFiltersModel {
  description: string | null;
  startDate: Date | null;
  endDate: Date | null;
  expenseTypes: ExpenseType[];
  transactionTypes: TransactionType[];
}
