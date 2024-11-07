import { ILoginRequestModel } from '@/features/login/models/loginRequestModel';
import { useForm } from 'react-hook-form';

export const useLogin = () => {
  const { control, formState, handleSubmit } = useForm<ILoginRequestModel>();

  return {
    control,
    handleSubmit,
  };
};
