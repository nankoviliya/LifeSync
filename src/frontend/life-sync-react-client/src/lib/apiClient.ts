import axios, { AxiosRequestConfig } from 'axios';

import { environment } from '@/config/currentEnvironment';
import { endpoints } from '@/config/endpoints/endpoints';

export interface ApiRequestConfig extends AxiosRequestConfig {
  skipAuthRefresh?: boolean;
}

export const apiClient = axios.create({
  baseURL: environment.xApiUrl,
  paramsSerializer: {
    indexes: true,
  },
});

apiClient.interceptors.request.use((config) => {
  if (config.headers) {
    config.headers.Accept = 'application/json';
  }

  config.withCredentials = true;
  return config;
});

let refreshPromise: Promise<void> | null = null;

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const config = error.config as ApiRequestConfig;

    if (error.response?.status !== 401 || config?.skipAuthRefresh) {
      return Promise.reject(error);
    }

    if (!refreshPromise) {
      refreshPromise = refreshToken()
        .catch((err) => {
          window.location.href = '/login?session_expired=true';
          throw err;
        })
        .finally(() => {
          refreshPromise = null;
        });
    }

    await refreshPromise;
    config.skipAuthRefresh = true;
    return apiClient(config);
  },
);

async function refreshToken(): Promise<void> {
  await apiClient.post(`/api/${endpoints.auth.refresh}`, null, {
    skipAuthRefresh: true,
  } as ApiRequestConfig);
}

export const get = async <TResponse>(
  path: string,
  config?: ApiRequestConfig,
) => {
  const response = await apiClient.get<TResponse>(path, {
    ...config,
  });

  return response.data;
};

export const post = async <TResponse, TData>(
  path: string,
  data: TData,
  config?: ApiRequestConfig,
) => {
  const response = await apiClient.post<TResponse>(path, data, {
    ...config,
  });

  return response.data as TResponse;
};

export const put = async <TResponse, TData>(
  path: string,
  data: TData,
  config?: ApiRequestConfig,
) => {
  const response = await apiClient.put<TResponse>(path, data, {
    ...config,
  });

  return response.data as TResponse;
};
