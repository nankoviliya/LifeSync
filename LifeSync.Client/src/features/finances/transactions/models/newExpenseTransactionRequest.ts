import { ExpenseType } from '@/features/finances/transactions/models/transactionsGetModel';

export interface INewExpenseTransactionRequest {
  amount: number;
  currency: string;
  date: Date;
  description: string;
  expenseType: ExpenseType;
}
