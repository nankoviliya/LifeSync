import { environment } from '@/environments/currentEnvironment';
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: environment.xApiUrl,
});

axiosInstance.interceptors.request.use(
  (config) => {
    // Retrieve the token from local storage or any storage mechanism you use
    const token = localStorage.getItem('token');

    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // Optionally, clear token and redirect to login page
      localStorage.removeItem('token');
    }

    return Promise.reject(error);
  },
);

export { axiosInstance };
