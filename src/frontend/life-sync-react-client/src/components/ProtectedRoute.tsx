import { Navigate, Outlet } from 'react-router-dom';

import { useAuth } from '@/hooks/useAuthentication';

export const ProtectedRoute = () => {
  const { isAuthenticated } = useAuth();

  return isAuthenticated ? <Outlet /> : <Navigate to="/" />;
};
