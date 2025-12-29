import { useMutation } from '@tanstack/react-query';

import { endpoints } from '@/config/endpoints/endpoints';
import { post } from '@/lib/apiClient';
import { useAuth } from '@/stores/AuthProvider';

export const useLogout = () => {
  const { logout: authLogout } = useAuth();

  const mutation = useMutation({
    mutationFn: () => post<void, object>(endpoints.auth.logout, {}),
    onSettled: authLogout, // runs on success OR error
  });

  return { logout: () => mutation.mutate() };
};
