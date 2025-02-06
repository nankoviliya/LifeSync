export const endpoints = {
  frontendSettings: {
    getFrontendSettings: 'FrontendSettings',
  },
  translations: {
    getTranslationsByLanguageId: `Translations`,
  },
  auth: {
    login: 'Auth/Login',
  },
  users: {
    getProfileData: 'Users/Profile',
    modifyProfileData: 'Users/Profile',
  },
  income: {
    getUserTransactions: 'Income/Transactions',
    addUserTransaction: 'Income',
  },
  expense: {
    getUserTransactions: 'Expenses/Transactions',
    addUserTransaction: 'Expenses',
  },
};
