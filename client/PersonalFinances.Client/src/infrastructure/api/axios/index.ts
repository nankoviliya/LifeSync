import { environment } from '@environments/currentEnvironment';
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: environment.xApiUrl,
});

export { axiosInstance };
