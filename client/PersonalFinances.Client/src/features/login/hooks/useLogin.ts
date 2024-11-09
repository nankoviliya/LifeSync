import { ILoginRequestModel } from '@/features/login/models/loginRequestModel';
import { ILoginResponseModel } from '@/features/login/models/loginResponseModel';
import { endpoints } from '@/infrastructure/api/endpoints/endpoints';
import { post } from '@/infrastructure/api/methods/post';
import { useAuth } from '@/infrastructure/authentication/hooks/useAuthentication';
import { routePaths } from '@/infrastructure/routing/routePaths';
import { useMutation } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';

export const useLogin = () => {
  const { control, handleSubmit } = useForm<ILoginRequestModel>();

  const { login: authLogin } = useAuth();
  const navigate = useNavigate();

  const mutation = useMutation({
    mutationFn: async (data: ILoginRequestModel) => {
      return post<ILoginResponseModel, ILoginRequestModel>(
        endpoints.auth.login,
        data,
      );
    },
    onSuccess: (data) => {
      authLogin(data.token);
      navigate(routePaths.home.path);
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit: SubmitHandler<ILoginRequestModel> = (data, event) => {
    mutation.mutate(data);
  };

  return {
    control,
    onSubmit: handleSubmit(onSubmit),
  };
};
