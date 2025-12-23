import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface UseUserProfileOptions {
  enabled?: boolean;
}

export const useUserProfile = (options?: UseUserProfileOptions) => {
  const { data, isLoading, isSuccess } = useReadQuery<IUserProfileDataModel>({
    endpoint: endpointsOptions.getUserAccountData.endpoint,
    queryKey: [endpointsOptions.getUserAccountData.key],
    staleTime: 86_400_000,
    enabled: options?.enabled,
  });

  return {
    data,
    isLoading,
    isSuccess,
  };
};
