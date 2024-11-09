import { INewExpenseTransactionRequest } from '@/features/expenseTracking/models/newExpenseTransactionRequest';
import { endpoints } from '@/infrastructure/api/endpoints/endpoints';
import { post } from '@/infrastructure/api/methods/post';
import { useMutation } from '@tanstack/react-query';
import { SubmitHandler } from 'react-hook-form';

export const useNewExpenseTransactionMutation = (closeModal: () => void) => {
  const mutation = useMutation({
    mutationFn: async (data: INewExpenseTransactionRequest) => {
      return post<any, INewExpenseTransactionRequest>(
        endpoints.expense.addUserTransaction,
        data,
      );
    },
    onSuccess: () => {
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
