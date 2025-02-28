export interface IExpenseTransactionsGetModel {
  expenseTransactions: IExpenseTransactionGetModel[];
  expenseSummary: IExpenseSummaryData;
  transactionsCount: number;
}

export interface IExpenseSummaryData {
  totalSpent: number;
  totalSpentOnNeeds: number;
  totalSpentOnWants: number;
  totalSpentOnSavings: number;
  currency: string;
}

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
