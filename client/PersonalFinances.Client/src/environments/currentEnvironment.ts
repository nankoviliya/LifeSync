import local_env_var from '@environments/environment.local';

const { VITE_APP_ENV: APP_ENV } = import.meta.env;

const getCurrentEnvironment = (env: string) => {
  switch (env) {
    case 'local':
      return local_env_var;
    default:
      return local_env_var;
  }
};

export const environment = getCurrentEnvironment(APP_ENV);
