import axios, { AxiosRequestConfig, InternalAxiosRequestConfig } from 'axios';

import { environment } from '@/config/currentEnvironment';

function authRequestInterceptor(config: InternalAxiosRequestConfig) {
  if (config.headers) {
    config.headers.Accept = 'application/json';
  }

  config.withCredentials = true;
  return config;
}

export const apiClient = axios.create({
  baseURL: environment.xApiUrl,
});

apiClient.interceptors.request.use(authRequestInterceptor);

apiClient.interceptors.request.use(
  (config) => {
    // Retrieve the token from local storage or any storage mechanism you use
    const token = localStorage.getItem('token');

    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(
      //Expected the Promise rejection reason to be an Error.
      error instanceof Error ? error : new Error(String(error)),
    );
  },
);

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // Optionally, clear token and redirect to login page
      localStorage.removeItem('token');
    }

    return Promise.reject(
      // Expected the Promise rejection reason to be an Error.
      error instanceof Error ? error : new Error(String(error)),
    );
  },
);

export const get = async <TResponse>(
  path: string,
  config?: AxiosRequestConfig,
) => {
  const response = await apiClient.get<TResponse>(path, {
    ...config,
  });

  return response.data;
};

export const post = async <TResponse, TData>(
  path: string,
  data: TData,
  config?: AxiosRequestConfig,
) => {
  const response = await apiClient.post<TResponse>(path, data, {
    ...config,
  });

  return response.data as TResponse;
};

export const put = async <TResponse, TData>(
  path: string,
  data: TData,
  config?: AxiosRequestConfig,
) => {
  const response = await apiClient.put<TResponse>(path, data, {
    ...config,
  });

  return response.data as TResponse;
};
