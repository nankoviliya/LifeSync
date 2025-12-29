import { useAuth } from '@/stores/AuthProvider';

export const useHeader = () => {
  const { isAuthenticated } = useAuth();

  return {
    isAuthenticated,
  };
};
