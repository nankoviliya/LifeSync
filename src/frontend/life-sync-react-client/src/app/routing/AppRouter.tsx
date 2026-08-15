import { ErrorBoundary } from 'react-error-boundary';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Navigate,
  Route,
  RouterProvider,
} from 'react-router-dom';

import { AppRoot } from '@/app/AppRoot';
import { RequireAuth } from '@/app/routing/RequireAuth';
import { RequireUnauth } from '@/app/routing/RequireUnauth';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';
import { AppShell } from '@/components/layouts/AppShell';
import { AuthShell } from '@/components/layouts/AuthShell';
import { routePaths } from '@/config/routing/routePaths';
import { Login } from '@/features/auth/login/components/Login';
import { Register } from '@/features/auth/register/components/Register';
import { Finances } from '@/features/finances/Finances';
import { Home } from '@/features/home/Home';
import { UserProfile } from '@/features/userProfile/components/UserProfile';

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
      {/* Guest routes inside AuthShell */}
      <Route
        element={
          <RequireUnauth>
            <AuthShell />
          </RequireUnauth>
        }
      >
        <Route path={routePaths.login.path} element={<Login />} />
        <Route path={routePaths.register.path} element={<Register />} />
      </Route>

      {/* Protected routes inside AppShell */}
      <Route
        element={
          <RequireAuth>
            <AppShell />
          </RequireAuth>
        }
      >
        <Route path={routePaths.app.path} element={<Home />} />
        <Route path={routePaths.home.path} element={<Home />} />
        <Route path={routePaths.userProfile.path} element={<UserProfile />} />
        <Route path={routePaths.finances.path} element={<Finances />} />
        <Route
          path={routePaths.financeTransactions.path}
          element={<Navigate to={routePaths.finances.path} replace />}
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
