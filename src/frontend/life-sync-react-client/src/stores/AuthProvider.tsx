import { createContext, PropsWithChildren, useContext, useMemo } from 'react';

import { endpointsOptions } from '@/config/endpoints/endpointsOptions';
import { useReadQuery } from '@/hooks/api/useReadQuery';
import { IUserProfileDataModel } from '@/types/userProfileDataModel';

interface AuthContextType {
  isAuthenticated: boolean;
  isLoading: boolean;
  user: IUserProfileDataModel | null;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

export const AuthProvider = ({ children }: PropsWithChildren) => {
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

  const value = useMemo(
    () => ({ isAuthenticated, isLoading, user: user ?? null }),
    [isAuthenticated, isLoading, user],
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
