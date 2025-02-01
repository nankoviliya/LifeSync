import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { useReadQuery } from '@/infrastructure/api/hooks/useReadQuery';
import { IFrontendSettings } from '@/infrastructure/frontendSettings/models/frontendSettings';

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
