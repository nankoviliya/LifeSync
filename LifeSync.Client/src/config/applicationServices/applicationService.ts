import { IRoutePath, routePaths } from '@/config/routing/routePaths';

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
  financeTransactions: {
    id: '115e6b1b-05c8-4c93-a891-514f2b8f4656',
    internalName: 'financeTransactions',
    displayName: 'Transactions',
    labelTranslationCode: 'finances-transactions-service-name',
    routePath: routePaths.financeTransactions,
    descriptionTranslationCode: 'finances-transactions-service-description',
  },
  // incomeTransactions: {
  //   id: '0c7eede2-2c5b-4197-83e2-1d070a67b663',
  //   internalName: 'incomeTransactions',
  //   displayName: 'Income Transactions',
  //   labelTranslationCode: 'income-transactions-service-name',
  //   routePath: routePaths.incomeTracking,
  //   descriptionTranslationCode: 'income-transactions-service-description',
  // },
} as const;
