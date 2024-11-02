import { useReadQuery } from '@infrastructure/api/hooks/useReadQuery';
import { endpointsOptions } from '@infrastructure/api/endpoints/endpointsOptions';
import { IExpenseTransactionGetModel } from '../models/expenseTransactionModel';

export const useExpenseTracking = () => {
  const { data, isLoading, isSuccess } = useReadQuery<
    IExpenseTransactionGetModel[]
  >({
    endpoint: endpointsOptions.getUserExpenseTransactions.endpoint,
    queryKey: [endpointsOptions.getUserExpenseTransactions.key],
    staleTime: 86_400_000,
  });

  return {
    data,
    isLoading,
    isSuccess,
  };
};
