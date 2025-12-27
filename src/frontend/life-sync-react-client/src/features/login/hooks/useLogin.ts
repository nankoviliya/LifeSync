import { useMutation } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';

import { endpoints } from '@/config/endpoints/endpoints';
import { routePaths } from '@/config/routing/routePaths';
import { ILoginRequestModel } from '@/features/login/models/loginRequestModel';
import { useAuth } from '@/hooks/useAuthentication';
import { post } from '@/lib/apiClient';

export const useLogin = () => {
  const { control, handleSubmit } = useForm<ILoginRequestModel>();

  const { login: authLogin } = useAuth();
  const navigate = useNavigate();

  const mutation = useMutation({
    mutationFn: async (data: ILoginRequestModel) => {
      return post<void, ILoginRequestModel>(endpoints.auth.login, data);
    },
    onSuccess: () => {
      authLogin();
      navigate(routePaths.home.path);
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
