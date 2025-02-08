import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

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
