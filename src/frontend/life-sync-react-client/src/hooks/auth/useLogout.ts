import { useMutation } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';

import { endpoints } from '@/config/endpoints/endpoints';
import { routePaths } from '@/config/routing/routePaths';
import { post } from '@/lib/apiClient';
import { useAuth } from '@/stores/AuthProvider';

export const useLogout = () => {
  const { logout: authLogout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    authLogout();
    navigate(routePaths.login.path);
  };

  const mutation = useMutation({
    mutationFn: () => post<void, object>(endpoints.auth.logout, {}),
    onSuccess: handleLogout,
    onError: handleLogout,
  });

  return {
    logout: () => mutation.mutate(),
  };
};
