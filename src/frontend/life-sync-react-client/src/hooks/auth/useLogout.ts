import { useMutation } from '@tanstack/react-query';

import { endpoints } from '@/config/endpoints/endpoints';
import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useQueryInvalidation } from '@/hooks/api/useQueryInvalidation';
import { post } from '@/lib/apiClient';

export const useLogout = () => {
  const invalidateQuery = useQueryInvalidation();

  const mutation = useMutation({
    mutationFn: () => post<void, object>(endpoints.auth.logout, {}),
    onSettled: () => {
      invalidateQuery({ queryKey: [endpointsOptions.getUserAccountData.key] });
    },
  });

  return { logout: () => mutation.mutate() };
};
