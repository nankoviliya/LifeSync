import { useMutation } from '@tanstack/react-query';
import { useForm, SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewExpenseTransactionRequest } from '@/features/finances/transactions/models/newExpenseTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useNewExpenseTransaction = (closeForm: () => void) => {
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

      closeForm();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const { isPending } = mutation;

  const onSubmit: SubmitHandler<INewExpenseTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    control,
    isSubmitting: isPending,
    handleSubmit: handleSubmit(onSubmit),
  };
};
