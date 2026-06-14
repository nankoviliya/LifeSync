import { PropsWithChildren } from 'react';
import { Navigate } from 'react-router-dom';

import { SkeletonLoader } from '@/components/loaders/SkeletonLoader';
import { routePaths } from '@/config/routing/routePaths';
import { useAuth } from '@/stores/AuthProvider';

export const RequireAuth = ({ children }: PropsWithChildren) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) return <SkeletonLoader />;
  if (!isAuthenticated) return <Navigate to={routePaths.login.path} replace />;
  return <>{children}</>;
};
