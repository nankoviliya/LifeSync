export interface IRoutePath {
  path: string;
  name: string;
}

export const routePaths = {
  home: {
    path: '/',
    name: 'Home',
  },
  incomeTracking: {
    path: '/income-tracking',
    name: 'Income Tracking',
  },
  expenseTracking: {
    path: '/expense-tracking',
    name: 'Expense Tracking',
  },
};
