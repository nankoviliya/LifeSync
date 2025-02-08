import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useNewExpenseTransaction } from '@/features/finances/expenseTracking/addTransaction/hooks/useNewExpenseTransaction';
import { IExpenseTransactionsGetModel } from '@/features/finances/expenseTracking/models/expenseTransactionModel';
import { useReadQuery } from '@/hooks/api/useReadQuery';

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
