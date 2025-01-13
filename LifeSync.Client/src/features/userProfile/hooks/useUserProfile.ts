import { IUserProfileDataModel } from '@/features/userProfile/models/userProfileDataModel';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';

export const useUserProfile = () => {
  const { data, isLoading, isSuccess } = useReadQuery<IUserProfileDataModel>({
    endpoint: endpointsOptions.getUserProfileData.endpoint,
    queryKey: [endpointsOptions.getUserProfileData.key],
    staleTime: 86_400_000,
  });

  return {
    data,
    isLoading,
    isSuccess,
  };
};
