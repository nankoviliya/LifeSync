import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewExpenseTransactionRequest } from '@/features/finances/transactions/models/newExpenseTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useNewExpenseTransaction = () => {
  const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

  const { control, handleSubmit } = useForm<INewExpenseTransactionRequest>({
    defaultValues: {
      amount: 0,
      currency: '',
      date: undefined,
      description: '',
      expenseType: undefined,
    },
  });

  const invalidateQuery = useQueryInvalidation();

  const mutation = useMutation({
    mutationFn: async (data: INewExpenseTransactionRequest) => {
      return post<any, INewExpenseTransactionRequest>(
        endpoints.finances.addUserExpenseTransaction,
        data,
      );
    },
    onSuccess: () => {
      invalidateQuery({
        queryKey: [endpointsOptions.getUserTransactions.key],
      });
      setIsModalVisible(false);
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit: SubmitHandler<INewExpenseTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    control,
    handleSubmit: handleSubmit(onSubmit),
    isModalVisible,
    setIsModalVisible,
  };
};
