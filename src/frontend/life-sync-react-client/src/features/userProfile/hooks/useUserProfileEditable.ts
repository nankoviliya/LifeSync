import { useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { IModifyUserProfileDataModel } from '@/features/userProfile/models/modifyUserProfileDataModel';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { put } from '@/lib/apiClient';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

export const useUserProfileEditable = (
  userData: IUserProfileDataModel,
  disableEditMode: () => void,
) => {
  const invalidateQuery = useQueryInvalidation();

  const { control, handleSubmit } = useForm<IModifyUserProfileDataModel>({
    defaultValues: {
      firstName: userData.firstName ?? '',
      lastName: userData.lastName ?? '',
      languageId: userData.language.id ?? '',
    },
  });

  const updateMutation = useMutation({
    mutationFn: async (data: IModifyUserProfileDataModel) => {
      return put<unknown, IModifyUserProfileDataModel>(
        endpoints.account.modifyAccountData,
        data,
      );
    },
    onSuccess: () => {
      invalidateQuery({
        queryKey: [endpointsOptions.getUserAccountData.key],
      });
      disableEditMode();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const { isPending } = updateMutation;

  const onSubmit = (data: IModifyUserProfileDataModel) => {
    updateMutation.mutate(data);
  };

  return {
    control,
    handleSubmit,
    onSubmit,
    isSubmitting: isPending,
  };
};
