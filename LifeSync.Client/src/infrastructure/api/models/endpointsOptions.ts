import { IReadQueryBaseOptions } from './readQueryBaseOptions';

export interface IEndpointsOptions {
  getBaseInfo: IReadQueryBaseOptions;
  getUserProfileData: IReadQueryBaseOptions;
  getUserIncomeTransactions: IReadQueryBaseOptions;
  getUserExpenseTransactions: IReadQueryBaseOptions;
}
