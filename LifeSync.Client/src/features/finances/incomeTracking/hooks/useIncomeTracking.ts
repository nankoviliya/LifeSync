import { useNewIncomeTransaction } from '@/features/finances/incomeTracking/addTransaction/hooks/useNewIncomeTransaction';
import { IIncomeTransactionsGetModel } from '@/features/finances/incomeTracking/models/incomeTransactionModel';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';

export const useIncomeTracking = () => {
  const { control, isModalVisible, setIsModalVisible, handleSubmit } =
    useNewIncomeTransaction();

  const { data, isLoading, isSuccess } =
    useReadQuery<IIncomeTransactionsGetModel>({
      endpoint: endpointsOptions.getUserIncomeTransactions.endpoint,
      queryKey: [endpointsOptions.getUserIncomeTransactions.key],
      staleTime: 86_400_000,
    });

  return {
    data,
    control,
    isLoading,
    isSuccess,
    isModalVisible,
    setIsModalVisible,
    handleSubmit,
  };
};
