import { useMutation } from '@tanstack/react-query';
import { useForm, SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewIncomeTransactionRequest } from '@/features/finances/transactions/models/newIncomeTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useNewIncomeTransaction = (closeForm: () => void) => {
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

      closeForm();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const { isPending } = mutation;

  const onSubmit: SubmitHandler<INewIncomeTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    control,
    isSubmitting: isPending,
    handleSubmit: handleSubmit(onSubmit),
  };
};
