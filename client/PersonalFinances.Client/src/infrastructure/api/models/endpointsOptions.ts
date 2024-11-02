import { IReadQueryBaseOptions } from './readQueryBaseOptions';

export interface IEndpointsOptions {
  getBaseInfo: IReadQueryBaseOptions;
  getUserIncomeTransactions: IReadQueryBaseOptions;
  getUserExpenseTransactions: IReadQueryBaseOptions;
}
