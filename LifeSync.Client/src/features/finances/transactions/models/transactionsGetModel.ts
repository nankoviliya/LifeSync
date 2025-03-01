export interface ITransactionsGetModel {
  transactions: IExpenseTransactionGetModel[] | IIncomeTransactionGetModel[];
  expenseSummary: IExpenseSummaryData;
  incomeSummary: IIncomeSummaryData;
  transactionsCount: number;
}

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

export enum TransactionType {
  Income = 'Income',
  Expense = 'Expense',
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

export enum ExpenseType {
  Needs = 'Needs',
  Wants = 'Wants',
  Savings = 'Savings',
}
