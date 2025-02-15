import { IReadQueryBaseOptions } from '@/types/readQueryBaseOptions';

export interface IEndpointsOptions {
  getFrontendSettings: IReadQueryBaseOptions;
  getTranslations: IReadQueryBaseOptions;
  getUserProfileData: IReadQueryBaseOptions;
  getUserIncomeTransactions: IReadQueryBaseOptions;
  getUserExpenseTransactions: IReadQueryBaseOptions;
}
