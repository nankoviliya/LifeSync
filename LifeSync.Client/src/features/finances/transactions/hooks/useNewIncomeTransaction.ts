import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewIncomeTransactionRequest } from '@/features/finances/transactions/models/newIncomeTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useNewIncomeTransaction = () => {
  const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

  const { control, handleSubmit } = useForm<INewIncomeTransactionRequest>({
    defaultValues: {
      amount: 0,
      currency: '',
      date: undefined,
      description: '',
    },
  });

  const invalidateQuery = useQueryInvalidation();

  const mutation = useMutation({
    mutationFn: async (data: INewIncomeTransactionRequest) => {
      return post<any, INewIncomeTransactionRequest>(
        endpoints.finances.addUserIncomeTransaction,
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

  const onSubmit: SubmitHandler<INewIncomeTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    control,
    handleSubmit: handleSubmit(onSubmit),
    isModalVisible,
    setIsModalVisible,
  };
};
