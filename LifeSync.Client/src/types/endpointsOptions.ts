import { IReadQueryBaseOptions } from '@/types/readQueryBaseOptions';

export interface IEndpointsOptions {
  getFrontendSettings: IReadQueryBaseOptions;
  getTranslations: IReadQueryBaseOptions;
  getUserAccountData: IReadQueryBaseOptions;
  getUserTransactions: IReadQueryBaseOptions;
  getUserIncomeTransactions: IReadQueryBaseOptions;
  getUserExpenseTransactions: IReadQueryBaseOptions;
}
