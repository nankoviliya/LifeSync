export enum ExpenseType {
  Needs = 'Needs',
  Wants = 'Wants',
  Savings = 'Savings',
}

export interface INewExpenseTransactionRequest {
  amount: number;
  currency: string;
  date: Date;
  description: string;
  expenseType: ExpenseType;
}
