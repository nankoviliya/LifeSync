import { PropsWithChildren } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Navigate,
  Outlet,
  Route,
  RouterProvider,
} from 'react-router-dom';

import { AppRoot } from '@/app/AppRoot';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';
import { routePaths } from '@/config/routing/routePaths';
import { Login } from '@/features/auth/login/components/Login';
import { Register } from '@/features/auth/register/components/Register';
import { Finances } from '@/features/finances/Finances';
import { Transactions } from '@/features/finances/transactions/components/Transactions';
import { Home } from '@/features/home/Home';
import { UserProfile } from '@/features/userProfile/components/UserProfile';
import { useAuth } from '@/stores/AuthProvider';

const ProtectedRoute = ({ children }: PropsWithChildren) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) return null; // or <LoadingSpinner />
  if (!isAuthenticated) return <Navigate to={routePaths.login.path} replace />;

  return children ?? <Outlet />;
};

const GuestRoute = ({ children }: PropsWithChildren) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) return null;
  if (isAuthenticated) return <Navigate to={routePaths.home.path} replace />;

  return children ?? <Outlet />;
};

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route
      element={
        <ErrorBoundary FallbackComponent={MainErrorFallback}>
          <AppRoot />
        </ErrorBoundary>
      }
      errorElement={<MainErrorFallback />}
    >
      {/* Guest routes */}
      <Route element={<GuestRoute />}>
        <Route path={routePaths.login.path} element={<Login />} />
        <Route path={routePaths.register.path} element={<Register />} />
      </Route>

      {/* Protected routes */}
      <Route element={<ProtectedRoute />}>
        <Route path={routePaths.app.path} element={<Home />} />
        <Route path={routePaths.home.path} element={<Home />} />
        <Route path={routePaths.userProfile.path} element={<UserProfile />} />
        <Route path={routePaths.finances.path} element={<Finances />} />
        <Route
          path={routePaths.financeTransactions.path}
          element={<Transactions />}
        />
      </Route>

      {/* Fallback */}
      <Route
        path="*"
        element={<Navigate to={routePaths.login.path} replace />}
      />
    </Route>,
  ),
);

export const AppRouter = () => <RouterProvider router={router} />;
