import { useMutation } from '@tanstack/react-query';
import { SubmitHandler, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';

import { endpoints } from '@/config/endpoints/endpoints';
import { routePaths } from '@/config/routing/routePaths';
import { IRegisterRequestModel } from '@/features/auth/register/models/registerRequestModel';
import { post } from '@/lib/apiClient';

export const useRegistration = () => {
  const { control, handleSubmit } = useForm<IRegisterRequestModel>({
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      balance: 0,
      currency: '',
      languageId: '',
    },
  });

  const navigate = useNavigate();

  const mutation = useMutation({
    mutationFn: async (data: IRegisterRequestModel) => {
      return post<unknown, IRegisterRequestModel>(
        endpoints.auth.register,
        data,
      );
    },
    onSuccess: () => {
      navigate(routePaths.login.path);
    },
    onError: () => {
      console.log('Registration error');
    },
  });

  const { isPending } = mutation;

  const onSubmit: SubmitHandler<IRegisterRequestModel> = (data) => {
    mutation.mutate(data);
  };

  return {
    control,
    onSubmit: handleSubmit(onSubmit),
    isSubmitting: isPending,
  };
};
