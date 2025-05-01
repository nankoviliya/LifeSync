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
