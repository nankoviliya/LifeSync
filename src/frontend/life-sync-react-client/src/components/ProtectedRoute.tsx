import { Navigate, Outlet } from 'react-router-dom';

import { useAuth } from '@/stores/AuthProvider';

export const ProtectedRoute = () => {
  const { isAuthenticated } = useAuth();

  return isAuthenticated ? <Outlet /> : <Navigate to="/" />;
};
