import { IMoney } from '@/infrastructure/common/models/money';

export interface IIncomeTransactionGetModel {
  id: string;
  amount: IMoney;
  date: string;
  description: string;
}
