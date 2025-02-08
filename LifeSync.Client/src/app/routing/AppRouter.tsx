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
import { Finances } from '@/features/finances/components/Finances';
import { ExpenseTracking } from '@/features/finances/expenseTracking/components/ExpenseTracking';
import { IncomeTracking } from '@/features/finances/incomeTracking/components/IncomeTracking';
import { Home } from '@/features/home/Home';
import { Login } from '@/features/login/components/Login';
import { Register } from '@/features/register/components/Register';
import { UserProfile } from '@/features/userProfile/components/UserProfile';
import { useAuth } from '@/hooks/useAuthentication';

export const AppRouter = () => {
  const { isAuthenticated } = useAuth();

  const protectedPages = (
    <>
      <Route path={routePaths.app.path} element={<Home />} />
      <Route path={routePaths.home.path} element={<Home />} />
      <Route path={routePaths.userProfile.path} element={<UserProfile />} />
      <Route path={routePaths.finances.path} element={<Finances />} />
      <Route
        path={routePaths.expenseTracking.path}
        element={<ExpenseTracking />}
      />
      <Route
        path={routePaths.incomeTracking.path}
        element={<IncomeTracking />}
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
          {isAuthenticated && protectedPages}
          <Route path={routePaths.login.path} element={<Login />} />
          <Route path={routePaths.register.path} element={<Register />} />
          {!isAuthenticated && (
            <Route
              path="*"
              element={<Navigate to={routePaths.login.path} replace />}
            />
          )}
          {isAuthenticated && <Route path="*" element={<>not found</>} />}
        </Route>
      </>,
    ),
  );

  return <RouterProvider router={router} />;
};
