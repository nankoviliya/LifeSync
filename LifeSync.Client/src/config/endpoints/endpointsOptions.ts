import { IEndpointsOptions } from '@/types/endpointsOptions';

import { endpoints } from './endpoints';

export const endpointsOptions: IEndpointsOptions = {
  getFrontendSettings: {
    endpoint: endpoints.frontendSettings.getFrontendSettings,
    key: 'frontend-settings',
  },
  getTranslations: {
    endpoint: endpoints.translations.getTranslationsByLanguageId,
    key: 'translations',
  },
  getUserAccountData: {
    endpoint: endpoints.account.getAccountData,
    key: 'user-account-data',
  },
  getUserTransactions: {
    endpoint: endpoints.finances.getUserTransactions,
    key: 'user-transactions',
  },
  getUserIncomeTransactions: {
    endpoint: endpoints.finances.getUserIncomeTransactions,
    key: 'user-income-transactions',
  },
  getUserExpenseTransactions: {
    endpoint: endpoints.finances.getUserExpenseTransactions,
    key: 'user-expense-transactions',
  },
};
