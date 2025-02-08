import { useAuth } from '@/hooks/useAuthentication';

export const useHeader = () => {
  const login = useAuth();

  const isUserAuthenticated = login.isAuthenticated;

  return {
    isUserAuthenticated,
  };
};
