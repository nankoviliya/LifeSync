import axios, { AxiosRequestConfig, InternalAxiosRequestConfig } from 'axios';

import { environment } from '@/config/currentEnvironment';
import { endpoints } from '@/config/endpoints/endpoints';

function getCookie(name: string): string | null {
  const matches = document.cookie.match(
    new RegExp(`(?:^|; )${name.replace(/([.$?*|{}()[\]\\/+^])/g, '\\$1')}=([^;]*)`)
  );
  return matches ? decodeURIComponent(matches[1]) : null;
}

function authRequestInterceptor(config: InternalAxiosRequestConfig) {
  if (config.headers) {
    config.headers.Accept = 'application/json';

    // Read CSRF token from cookie and add to header
    const csrfToken = getCookie('csrf_token');
    if (csrfToken) {
      config.headers['X-CSRF-TOKEN'] = csrfToken;
    }
  }

  config.withCredentials = true;
  return config;
}

export const apiClient = axios.create({
  baseURL: environment.xApiUrl,
  paramsSerializer: {
    indexes: true,
  },
});

apiClient.interceptors.request.use(authRequestInterceptor);

// Token refresh logic
let isRefreshing = false;
let failedQueue: Array<{ resolve: (value?: unknown) => void; reject: (reason?: unknown) => void }> = [];

function processQueue(error: unknown) {
  failedQueue.forEach((promise) => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve();
    }
  });

  failedQueue = [];
}

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // Queue this request until refresh completes
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(() => apiClient(originalRequest))
          .catch((err) => Promise.reject(err instanceof Error ? err : new Error(String(err))));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        await apiClient.post(`/api/${endpoints.auth.refresh}`);
        processQueue(null);
        return apiClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError);
        window.location.href = '/login?session_expired=true';
        return Promise.reject(
          refreshError instanceof Error ? refreshError : new Error(String(refreshError))
        );
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(
      error instanceof Error ? error : new Error(String(error))
    );
  }
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
