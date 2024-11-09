export interface IExpenseTransactionGetModel {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description: string;
  expenseType: ExpenseType;
}

export enum ExpenseType {
  Needs = 'Needs',
  Wants = 'Wants',
  Savings = 'Savings',
}
