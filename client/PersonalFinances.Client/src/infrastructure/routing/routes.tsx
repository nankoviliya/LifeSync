import App from '@/App';
import { ExpenseTracking } from '@/features/expenseTracking/components/ExpenseTracking';
import { IncomeTracking } from '@/features/incomeTracking/components/IncomeTracking';
import { routePaths } from '@/infrastructure/routing/routePaths';
import * as React from 'react';
import { RouteObject } from 'react-router-dom';

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
