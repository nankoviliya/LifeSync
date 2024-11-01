import { EnvironmentDefaultUrlType } from '@infrastructure/common/enums/environmentDefaultUrlType';

export interface IEnvironment {
  version: string;
  defaultUrl: EnvironmentDefaultUrlType;
  xApiUrl: string;
}
