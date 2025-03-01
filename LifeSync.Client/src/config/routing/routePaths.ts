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
  register: {
    path: '/register',
    name: 'Sign Up',
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
  financeTransactions: {
    path: '/finances/transactions',
    name: 'Transactions',
  },
} as const;
