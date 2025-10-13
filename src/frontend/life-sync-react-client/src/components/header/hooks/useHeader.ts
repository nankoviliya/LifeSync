import { useAuth } from '@/hooks/useAuthentication';

export const useHeader = () => {
  const { isAuthenticated } = useAuth();

  return {
    isUserAuthenticated: isAuthenticated,
  };
};
