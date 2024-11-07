import { EnvironmentDefaultUrlType } from "@/infrastructure/common/enums/environmentDefaultUrlType";
import { IEnvironment } from "@/infrastructure/common/models/environment";

const { VITE_APP_API_URL: API_URL } = import.meta.env;

const localEnvironment: IEnvironment = {
  version: '1.0.0',
  defaultUrl: EnvironmentDefaultUrlType.Local,
  xApiUrl: API_URL,
};

export default localEnvironment;
