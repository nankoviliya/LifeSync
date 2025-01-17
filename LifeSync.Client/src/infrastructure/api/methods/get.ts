import { axiosInstance } from '@/infrastructure/api/axios';
import { AxiosRequestConfig } from 'axios';

export const get = async <TResponse>(
  path: string,
  config?: AxiosRequestConfig,
) => {
  const response = await axiosInstance.get<TResponse>(path, {
    ...config,
  });

  return response.data;
};
