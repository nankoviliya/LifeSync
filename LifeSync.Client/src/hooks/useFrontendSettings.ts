import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { IFrontendSettings } from '@/types/frontendSettings';

export const useFrontendSettings = () => {
  const { data, isLoading, isSuccess } = useReadQuery<IFrontendSettings>({
    endpoint: endpointsOptions.getFrontendSettings.endpoint,
    queryKey: [endpointsOptions.getFrontendSettings.key],
    staleTime: Infinity,
  });

  return {
    frontendSettings: data,
    isLoading,
    isSuccess,
  };
};
