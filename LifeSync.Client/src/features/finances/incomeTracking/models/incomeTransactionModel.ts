
export interface IIncomeTransactionsGetModel {
  incomeTransactions: IIncomeTransactionGetModel[];
}

export interface IIncomeTransactionGetModel {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description: string;
}
