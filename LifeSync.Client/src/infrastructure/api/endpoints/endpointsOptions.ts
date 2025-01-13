import { IEndpointsOptions } from '@/infrastructure/api/models/endpointsOptions';
import { endpoints } from './endpoints';

export const endpointsOptions: IEndpointsOptions = {
  getBaseInfo: {
    endpoint: endpoints.base.getBaseInfo,
    key: 'base-info',
  },
  getUserProfileData: {
    endpoint: endpoints.users.getProfileData,
    key: 'user-profile-data',
  },
  getUserIncomeTransactions: {
    endpoint: endpoints.income.getUserTransactions,
    key: 'user-income-transactions',
  },
  getUserExpenseTransactions: {
    endpoint: endpoints.expense.getUserTransactions,
    key: 'user-expense-transactions',
  },
};
