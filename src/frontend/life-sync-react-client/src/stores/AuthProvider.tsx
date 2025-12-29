import { useQueryClient } from '@tanstack/react-query';
import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useMemo,
} from 'react';

import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface AuthContextType {
  login: () => void;
  logout: () => void;
  isAuthenticated: boolean;
  isLoading: boolean;
  user: IUserProfileDataModel | null;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

export const AuthProvider = ({ children }: PropsWithChildren) => {
  const queryClient = useQueryClient();

  const {
    data: user,
    isLoading,
    isSuccess,
  } = useReadQuery<IUserProfileDataModel>({
    endpoint: endpointsOptions.getUserAccountData.endpoint,
    queryKey: [endpointsOptions.getUserAccountData.key],
    staleTime: 86_400_000,
    retry: false,
    config: { skipAuthRefresh: true },
  });

  const isAuthenticated = isSuccess && user !== null;

  const login = useCallback(() => {
    queryClient.invalidateQueries({
      queryKey: [endpointsOptions.getUserAccountData.key],
    });
  }, [queryClient]);

  const logout = useCallback(() => {
    queryClient.clear();
  }, [queryClient]);

  const value = useMemo(
    () => ({ login, logout, isAuthenticated, isLoading, user: user ?? null }),
    [login, logout, isAuthenticated, isLoading, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }

  return context;
};
