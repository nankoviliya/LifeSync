import { EnvironmentDefaultUrlType } from '@/types/environmentDefaultUrlType';

export interface IEnvironment {
  version: string;
  defaultUrl: EnvironmentDefaultUrlType;
  xApiUrl: string;
}
