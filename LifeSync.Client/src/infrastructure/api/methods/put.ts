import { axiosInstance } from '@/infrastructure/api/axios';
import { AxiosRequestConfig } from 'axios';

export const put = async <TResponse, TData>(
  path: string,
  data: any,
  config?: AxiosRequestConfig,
) => {
  const response = await axiosInstance.put<TResponse>(path, data, {
    ...config,
  });

  return response.data;
};
