import { useMutation } from '@tanstack/react-query';
import { SubmitHandler } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { INewIncomeTransactionRequest } from '@/features/finances/incomeTracking/addTransaction/models/newIncomeTransactionRequest';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

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
