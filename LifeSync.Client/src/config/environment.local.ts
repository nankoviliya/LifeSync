import { IEnvironment } from '@/types/environment';
import { EnvironmentDefaultUrlType } from '@/types/environmentDefaultUrlType';

const { VITE_APP_API_URL: API_URL } = import.meta.env;

const localEnvironment: IEnvironment = {
  version: '1.0.0',
  defaultUrl: EnvironmentDefaultUrlType.Local,
  xApiUrl: API_URL,
};

export default localEnvironment;
