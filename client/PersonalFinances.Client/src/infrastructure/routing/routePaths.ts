export interface IRoutePath {
  path: string;
  name: string;
}

export const routePaths = {
  app: {
    path: '/',
    name: 'App',
  },
  login: {
    path: '/login',
    name: 'Login',
  },
  home: {
    path: '/home',
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
