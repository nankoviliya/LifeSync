import { IRoutePath, routePaths } from '@/infrastructure/routing/routePaths';

export interface IApplicationService {
  id: string;
  internalName: string;
  displayName: string;
  labelTranslationCode: string;
  descriptionTranslationCode: string;
  routePath: IRoutePath;
  parentService?: IApplicationService;
}

export const applicationServices = {
  finances: {
    id: '64683dca-4f89-4e57-a069-0bf0377e5d26',
    internalName: 'finances',
    displayName: 'Finances',
    labelTranslationCode: 'finances-service-name',
    routePath: routePaths.finances,
    descriptionTranslationCode: 'finances-service-description',
  },
  expenseTransactions: {
    id: '115e6b1b-05c8-4c93-a891-514f2b8f4656',
    internalName: 'expenseTransactions',
    displayName: 'Expense Transactions',
    labelTranslationCode: 'expense-transactions-service-name',
    routePath: routePaths.expenseTracking,
    descriptionTranslationCode: 'expense-transactions-service-description',
  },
  incomeTransactions: {
    id: '0c7eede2-2c5b-4197-83e2-1d070a67b663',
    internalName: 'incomeTransactions',
    displayName: 'Income Transactions',
    labelTranslationCode: 'income-transactions-service-name',
    routePath: routePaths.incomeTracking,
    descriptionTranslationCode: 'income-transactions-service-description',
  },
} as const;
