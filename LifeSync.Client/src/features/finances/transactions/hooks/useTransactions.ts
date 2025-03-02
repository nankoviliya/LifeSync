import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { ITransactionsFiltersModel } from '@/features/finances/transactions/models/transactionsFiltersModel';
import { ITransactionsGetModel } from '@/features/finances/transactions/models/transactionsGetModel';
import { useReadQuery } from '@/hooks/api/useReadQuery';

export const useTransactions = (filters: ITransactionsFiltersModel) => {
  const { data, isLoading, isSuccess, refetch } =
    useReadQuery<ITransactionsGetModel>({
      endpoint: endpointsOptions.getUserTransactions.endpoint,
      queryKey: [endpointsOptions.getUserTransactions.key],
      config: { params: filters },
      staleTime: 86_400_000,
    });

  return {
    data,
    isLoading,
    isSuccess,
    refetch,
  };
};
