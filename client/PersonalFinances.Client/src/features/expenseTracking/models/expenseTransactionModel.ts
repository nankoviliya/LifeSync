import { IMoney } from '@infrastructure/common/models/money';

export interface IExpenseTransactionGetModel {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description: string;
  expenseType: ExpenseType;
}

export enum ExpenseType {
  Needs,
  Wants,
  Savings,
}
