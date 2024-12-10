import { IMoney } from '@/infrastructure/common/models/money';

export interface IIncomeTransactionGetModel {
  id: string;
  amount: number;
  currency: string;
  date: string;
  description: string;
}
