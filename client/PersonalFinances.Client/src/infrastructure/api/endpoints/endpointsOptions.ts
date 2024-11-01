import { IEndpointsOptions } from '@infrastructure/api/models/endpointsOptions';
import { endpoints } from './endpoints';

export const endpointsOptions: IEndpointsOptions = {
  getBaseInfo: {
    endpoint: endpoints.base.getBaseInfo,
    key: 'base-info',
  },
};
