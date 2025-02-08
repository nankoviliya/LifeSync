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
  getUserIncomeTransactions: {
    endpoint: endpoints.income.getUserTransactions,
    key: 'user-income-transactions',
  },
  getUserExpenseTransactions: {
    endpoint: endpoints.expense.getUserTransactions,
    key: 'user-expense-transactions',
  },
};
