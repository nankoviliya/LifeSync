import { QueryKey, useQuery, UseQueryOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { get } from '@/lib/apiClient';

interface IUseReadQueryOptions {
  endpoint: string;
  queryKey: QueryKey;
}

export const useReadQuery = <TResponse>(
  options: IUseReadQueryOptions & UseQueryOptions<TResponse, AxiosError>,
) => {
  const queryFn = () => get<TResponse>(options.endpoint);

  const query = useQuery({ queryFn, ...options });

  return query;
};
