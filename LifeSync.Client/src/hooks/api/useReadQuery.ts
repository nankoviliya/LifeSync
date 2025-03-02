import { QueryKey, useQuery, UseQueryOptions } from '@tanstack/react-query';
import { AxiosError, AxiosRequestConfig } from 'axios';

import { get } from '@/lib/apiClient';

interface IUseReadQueryOptions {
  endpoint: string;
  queryKey: QueryKey;
  config?: AxiosRequestConfig;
}

export const useReadQuery = <TResponse>(
  options: IUseReadQueryOptions & UseQueryOptions<TResponse, AxiosError>,
) => {
  const { endpoint, queryKey, config, ...queryOptions } = options;

  const queryFn = () => get<TResponse>(endpoint, config);

  const query = useQuery({ queryKey, queryFn, ...queryOptions });

  return query;
};
