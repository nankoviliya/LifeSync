import { IIncomeTransactionGetModel } from '@/features/incomeTracking/models/incomeTransactionModel';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';

export const useIncomeTracking = () => {
  const { data, isLoading, isSuccess } = useReadQuery<
    IIncomeTransactionGetModel[]
  >({
    endpoint: endpointsOptions.getUserIncomeTransactions.endpoint,
    queryKey: [endpointsOptions.getUserIncomeTransactions.key],
    staleTime: 86_400_000,
  });

  return {
    data,
    isLoading,
    isSuccess,
  };
};
