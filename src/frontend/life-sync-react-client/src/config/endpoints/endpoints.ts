export const endpoints = {
  frontendSettings: {
    getFrontendSettings: 'FrontendSettings',
  },
  translations: {
    getTranslationsByLanguageId: `Translations`,
  },
  auth: {
    login: 'Auth/Login',
    register: 'Auth/Register',
    logout: 'Auth/Logout',
    refresh: 'Auth/Refresh',
  },
  account: {
    getAccountData: 'Account',
    modifyAccountData: 'Account',
  },
  finances: {
    getUserTransactions: 'Finances/Transactions',
    getUserIncomeTransactions: 'Finances/Transactions/Income',
    addUserIncomeTransaction: 'Finances/Transactions/Income',
    getUserExpenseTransactions: 'Finances/Transactions/Expense',
    addUserExpenseTransaction: 'Finances/Transactions/Expense',
  },
};
