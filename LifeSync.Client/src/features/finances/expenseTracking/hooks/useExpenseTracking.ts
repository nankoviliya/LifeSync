import { useNewExpenseTransaction } from '@/features/finances/expenseTracking/addTransaction/hooks/useNewExpenseTransaction';
import { IExpenseTransactionsGetModel } from '@/features/finances/expenseTracking/models/expenseTransactionModel';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';

export const useExpenseTracking = () => {
  const { control, isModalVisible, setIsModalVisible, handleSubmit } =
    useNewExpenseTransaction();

  const { data, isLoading, isSuccess } =
    useReadQuery<IExpenseTransactionsGetModel>({
      endpoint: endpointsOptions.getUserExpenseTransactions.endpoint,
      queryKey: [endpointsOptions.getUserExpenseTransactions.key],
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
