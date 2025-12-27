import { useQueryClient } from '@tanstack/react-query';
import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';

import { SkeletonLoader } from '@/components/loaders/SkeletonLoader';
import { endpoints } from '@/config/endpoints/endpoints';
import { apiClient } from '@/lib/apiClient';

interface AuthContextType {
  login: () => void;
  logout: () => Promise<void>;
  isAuthenticated: boolean;
  isInitialized: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

export interface IAuthProviderProps {
  children: React.ReactNode;
}

export const AuthProvider = ({ children }: IAuthProviderProps) => {
  const queryClient = useQueryClient();

  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isInitialized, setIsInitialized] = useState<boolean>(false);

  const checkAuthStatus = useCallback(async () => {
    try {
      await apiClient.get(`/api/${endpoints.auth.status}`);
      setIsAuthenticated(true);
    } catch {
      setIsAuthenticated(false);
    } finally {
      setIsInitialized(true);
    }
  }, []);

  useEffect(() => {
    checkAuthStatus();
  }, [checkAuthStatus]);

  const login = useCallback(() => {
    setIsAuthenticated(true);
    queryClient.clear();
  }, [queryClient]);

  const logout = useCallback(async () => {
    try {
      await apiClient.post(`/api/${endpoints.auth.logout}`);
    } catch {
      // Ignore errors during logout
    } finally {
      setIsAuthenticated(false);
      queryClient.clear();
    }
  }, [queryClient]);

  // React Context Provider values should have stable identities typescript:S6481
  const contextValue = useMemo(
    () => ({
      login,
      logout,
      isAuthenticated,
      isInitialized,
    }),
    [login, logout, isAuthenticated, isInitialized],
  );

  if (!isInitialized) {
    return <SkeletonLoader />;
  }

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};
