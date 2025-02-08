import { App } from '@/features/app/App';
import { ExpenseTracking } from '@/features/finances/expenseTracking/components/ExpenseTracking';
import { Home } from '@/features/home/Home';
import { IncomeTracking } from '@/features/finances/incomeTracking/components/IncomeTracking';
import { Login } from '@/features/login/components/Login';
import { useAuth } from '@/infrastructure/authentication/hooks/useAuthentication';
import { routePaths } from '@/infrastructure/routing/routePaths';
import {
  createBrowserRouter,
  createRoutesFromElements,
  Navigate,
  Route,
  RouterProvider,
} from 'react-router-dom';
import { Finances } from '@/features/finances/components/Finances';
import { UserProfile } from '@/features/userProfile/components/UserProfile';
import { Register } from '@/features/register/components/Register';

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
        <Route key="app" element={<App />}>
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
