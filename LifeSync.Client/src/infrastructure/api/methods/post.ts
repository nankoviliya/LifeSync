import { axiosInstance } from '@/infrastructure/api/axios';
import { AxiosRequestConfig } from 'axios';

export const post = async <TResponse, TData>(
  path: string,
  data: any,
  config?: AxiosRequestConfig,
) => {
  const response = await axiosInstance.post<TResponse>(path, data, {
    ...config,
  });

  return response.data;
};
