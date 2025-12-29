import { useMutation } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export interface ILoginRequestModel {
  email: string;
  password: string;
}

export const useLogin = () => {
  const invalidateQuery = useQueryInvalidation();
  const { control, handleSubmit } = useForm<ILoginRequestModel>();

  const mutation = useMutation({
    mutationFn: async (data: ILoginRequestModel) => {
      return post<void, ILoginRequestModel>(endpoints.auth.login, data);
    },
    onSuccess: () => {
      invalidateQuery({ queryKey: [endpointsOptions.getUserAccountData.key] });
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit: SubmitHandler<ILoginRequestModel> = (data) => {
    mutation.mutate(data);
  };

  const { isPending } = mutation;

  return {
    control,
    onSubmit: handleSubmit(onSubmit),
    isLoginPending: isPending,
  };
};
