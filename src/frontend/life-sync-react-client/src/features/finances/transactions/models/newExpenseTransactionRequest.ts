import { ExpenseType } from '@/types/transactionTypes';

export interface INewExpenseTransactionRequest {
  amount: number;
  currency: string;
  date: Date;
  description: string;
  expenseType: ExpenseType;
}
