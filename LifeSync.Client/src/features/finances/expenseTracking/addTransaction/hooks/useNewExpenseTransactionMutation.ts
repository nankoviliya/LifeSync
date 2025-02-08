import { useMutation } from '@tanstack/react-query';
import { SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewExpenseTransactionRequest } from '@/features/finances/expenseTracking/addTransaction/models/newExpenseTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useNewExpenseTransactionMutation = (closeModal: () => void) => {
  const invalidateQuery = useQueryInvalidation();

  const mutation = useMutation({
    mutationFn: async (data: INewExpenseTransactionRequest) => {
      return post<any, INewExpenseTransactionRequest>(
        endpoints.expense.addUserTransaction,
        data,
      );
    },
    onSuccess: () => {
      invalidateQuery({
        queryKey: [endpointsOptions.getUserExpenseTransactions.key],
      });
      closeModal();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit: SubmitHandler<INewExpenseTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    onSubmit,
  };
};
