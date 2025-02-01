import { api } from '@/infrastructure/api';
import { endpointsOptions } from '@/infrastructure/api/endpoints/endpointsOptions';
import { queryClient } from '@/infrastructure/api/queryClient/queryClient';
import { IFrontendSettings } from '@/infrastructure/frontendSettings/models/frontendSettings';

export const prefetchAll = async (): Promise<void> => {
  await Promise.all([prefetchFrontendSettings()]);
};

const prefetchFrontendSettings = async (): Promise<void> => {
  await queryClient.prefetchQuery<IFrontendSettings>({
    queryKey: [endpointsOptions.getFrontendSettings.key],
    queryFn: async () => {
      const data = await api.GET<IFrontendSettings>(
        endpointsOptions.getFrontendSettings.endpoint,
      );

      return data;
    },
  });
};
