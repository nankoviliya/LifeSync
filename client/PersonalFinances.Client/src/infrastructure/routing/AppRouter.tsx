import { App } from '@/App';
import { ExpenseTracking } from '@/features/expenseTracking/components/ExpenseTracking';
import { Home } from '@/features/home/Home';
import { IncomeTracking } from '@/features/incomeTracking/components/IncomeTracking';
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

export const AppRouter = () => {
  const { isAuthenticated } = useAuth();

  const protectedPages = (
    <>
      <Route path={routePaths.home.path} element={<Home />} />
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
