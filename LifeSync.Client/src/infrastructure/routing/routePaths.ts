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
  userProfile: {
    path: '/profile',
    name: 'User Profile',
  },
  finances: {
    path: '/finances',
    name: 'Finances',
  },
  incomeTracking: {
    path: '/finances/income-tracking',
    name: 'Income Tracking',
  },
  expenseTracking: {
    path: '/finances/expense-tracking',
    name: 'Expense Tracking',
  },
} as const;
