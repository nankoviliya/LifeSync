import { IModifyUserProfileDataModel } from '@/features/userProfile/models/modifyUserProfileDataModel';
import { IUserProfileDataModel } from '@/features/userProfile/models/userProfileDataModel';
import { api } from '@/infrastructure/api';
import { endpoints } from '@/infrastructure/api/endpoints/endpoints';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useQueryInvalidation } from '@/infrastructure/api/hooks/useQueryInvalidation';
import { useMutation } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';

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
      return api.PUT<{}, IModifyUserProfileDataModel>(
        endpoints.users.modifyProfileData,
        data,
      );
    },
    onSuccess: (data) => {
      invalidateQuery({
        queryKey: [endpointsOptions.getUserProfileData.key],
      });
      disableEditMode();
    },
    onError: () => {
      console.log('Auth error');
    },
  });

  const onSubmit = (data: IModifyUserProfileDataModel) => {
    updateMutation.mutate(data);
  };

  return {
    control,
    handleSubmit,
    onSubmit,
  };
};
