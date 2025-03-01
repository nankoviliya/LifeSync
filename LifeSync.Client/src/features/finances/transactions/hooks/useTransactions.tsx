import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { ITransactionsGetModel } from '@/features/finances/transactions/models/transactionsGetModel';
import { useReadQuery } from '@/hooks/api/useReadQuery';

export const useTransactions = () => {
  const { data, isLoading, isSuccess } = useReadQuery<ITransactionsGetModel>({
    endpoint: endpointsOptions.getUserTransactions.endpoint,
    queryKey: [endpointsOptions.getUserTransactions.key],
    staleTime: 86_400_000,
  });

  console.log(data);

  return {
    data,
    isLoading,
    isSuccess,
  };
};
