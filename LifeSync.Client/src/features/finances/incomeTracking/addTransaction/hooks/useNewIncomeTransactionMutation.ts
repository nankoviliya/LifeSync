import { INewIncomeTransactionRequest } from '@/features/finances/incomeTracking/addTransaction/models/newIncomeTransactionRequest';
import { endpoints } from '@/infrastructure/api/endpoints/endpoints';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useQueryInvalidation } from '@/infrastructure/api/hooks/useQueryInvalidation';
import { post } from '@/infrastructure/api/methods/post';
import { useMutation } from '@tanstack/react-query';
import { SubmitHandler } from 'react-hook-form';

export const useNewIncomeTransactionMutation = (closeModal: () => void) => {
  const invalidateQuery = useQueryInvalidation();

  const mutation = useMutation({
    mutationFn: async (data: INewIncomeTransactionRequest) => {
      return post<any, INewIncomeTransactionRequest>(
        endpoints.income.addUserTransaction,
        data,
      );
    },
    onSuccess: () => {
      invalidateQuery({
        queryKey: [endpointsOptions.getUserIncomeTransactions.key],
      });
      closeModal();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit: SubmitHandler<INewIncomeTransactionRequest> = (data) => {
    mutation.mutate(data);
  };

  return {
    onSubmit,
  };
};
