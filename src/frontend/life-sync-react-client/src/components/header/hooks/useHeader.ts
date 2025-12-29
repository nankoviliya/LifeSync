import { useAuth } from '@/stores/AuthProvider';

export const useHeader = () => {
  const login = useAuth();

  const isUserAuthenticated = login.isAuthenticated;

  return {
    isUserAuthenticated,
  };
};
