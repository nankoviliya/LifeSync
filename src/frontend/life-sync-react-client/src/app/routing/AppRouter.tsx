import { ErrorBoundary } from 'react-error-boundary';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Navigate,
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

export const AppRouter = () => {
  const { isAuthenticated } = useAuth();

  const protectedPages = (
    <>
      <Route path={routePaths.app.path} element={<Home />} />
      <Route path={routePaths.home.path} element={<Home />} />
      <Route path={routePaths.userProfile.path} element={<UserProfile />} />
      <Route path={routePaths.finances.path} element={<Finances />} />
      <Route
        path={routePaths.financeTransactions.path}
        element={<Transactions />}
      />
    </>
  );

  const router = createBrowserRouter(
    createRoutesFromElements(
      <>
        <Route
          key="app"
          element={
            <ErrorBoundary FallbackComponent={MainErrorFallback}>
              <AppRoot />
            </ErrorBoundary>
          }
          errorElement={<MainErrorFallback />}
        >
          <Route
            path={routePaths.login.path}
            element={
              isAuthenticated ? (
                <Navigate to={routePaths.home.path} replace />
              ) : (
                <Login />
              )
            }
          />
          <Route
            path={routePaths.register.path}
            element={
              isAuthenticated ? (
                <Navigate to={routePaths.home.path} replace />
              ) : (
                <Register />
              )
            }
          />
          {!isAuthenticated && (
            <Route
              path="*"
              element={<Navigate to={routePaths.login.path} replace />}
            />
          )}
          {isAuthenticated && protectedPages}
          {isAuthenticated && <Route path="*" element={<>not found</>} />}
        </Route>
      </>,
    ),
  );

  return <RouterProvider router={router} />;
};
