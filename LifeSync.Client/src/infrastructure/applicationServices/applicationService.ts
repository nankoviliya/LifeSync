import { IRoutePath, routePaths } from '@/infrastructure/routing/routePaths';

export interface IApplicationService {
  id: string;
  internalName: string;
  displayName: string;
  translationCode: string;
  description: string;
  routePath: IRoutePath;
  parentService?: IApplicationService;
}

export const applicationServices = {
  finances: {
    id: '64683dca-4f89-4e57-a069-0bf0377e5d26',
    internalName: 'finances',
    displayName: 'Finances',
    translationCode: 'serviceNames:finances-service-name',
    routePath: routePaths.finances,
    description: 'Here you can manage your income and expense transactions.',
  },
  expenseTransactions: {
    id: '115e6b1b-05c8-4c93-a891-514f2b8f4656',
    internalName: 'expenseTransactions',
    displayName: 'Expense Transactions',
    translationCode: 'serviceNames:expense-transactions-service-name',
    routePath: routePaths.expenseTracking,
    description: 'Manage your expense transactions.',
  },
  incomeTransactions: {
    id: '0c7eede2-2c5b-4197-83e2-1d070a67b663',
    internalName: 'incomeTransactions',
    displayName: 'Income Transactions',
    translationCode: 'serviceNames:income-transactions-service-name',
    routePath: routePaths.incomeTracking,
    description: 'Manage your income transactions.',
  },
} as const;
