import localEnvironment from '@/environments/environment.local';

const { VITE_APP_ENV: APP_ENV } = import.meta.env;

const getCurrentEnvironment = (env: string) => {
  switch (env) {
    case 'local':
      return localEnvironment;
    default:
      return localEnvironment;
  }
};

export const environment = getCurrentEnvironment(APP_ENV);
