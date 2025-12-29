import { useQueryClient } from '@tanstack/react-query';
import {
  createContext,
  PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';

import { SkeletonLoader } from '@/components/loaders/SkeletonLoader';
import { endpoints } from '@/config/endpoints/endpoints';
import { post } from '@/lib/apiClient';

interface AuthContextType {
  login: () => void;
  logout: () => void;
  isAuthenticated: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

export const AuthProvider = ({ children }: PropsWithChildren) => {
  const queryClient = useQueryClient();
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isInitialized, setIsInitialized] = useState<boolean>(false);

  useEffect(() => {
    post(`${endpoints.auth.refresh}`, {}, { skipAuthRefresh: true })
      .then(() => setIsAuthenticated(true))
      .catch(() => setIsAuthenticated(false))
      .finally(() => setIsInitialized(true));
  }, []);

  const login = useCallback(() => {
    setIsAuthenticated(true);
    queryClient.clear();
  }, [queryClient]);

  const logout = useCallback(() => {
    setIsAuthenticated(false);
    queryClient.clear();
  }, [queryClient]);

  const value = useMemo(
    () => ({ login, logout, isAuthenticated }),
    [login, logout, isAuthenticated],
  );

  if (!isInitialized) {
    return <SkeletonLoader />;
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
