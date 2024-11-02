import App from '@App';
import { RouteObject } from 'react-router-dom';
import { routePaths } from '@infrastructure/routing/routePaths';
import { IncomeTracking } from '@features/incomeTracking/components/IncomeTracking';
import { ExpenseTracking } from '@features/expenseTracking/components/ExpenseTracking';

export const routes: RouteObject[] = [
  {
    path: routePaths.home.path,
    element: <App />,
  },
  {
    path: routePaths.expenseTracking.path,
    element: <ExpenseTracking />,
  },
  {
    path: routePaths.incomeTracking.path,
    element: <IncomeTracking />,
  },
];
