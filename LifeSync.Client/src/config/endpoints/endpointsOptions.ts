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
  getUserProfileData: {
    endpoint: endpoints.users.getProfileData,
    key: 'user-profile-data',
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
