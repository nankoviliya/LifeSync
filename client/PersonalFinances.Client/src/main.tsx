import { StrictMode } from 'react';
import './index.css';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './infrastructure/api/queryClient/queryClient';
import { createRoot } from 'react-dom/client';
import { PrimeReactProvider } from 'primereact/api';

import 'primereact/resources/themes/lara-light-blue/theme.css'; // Theme (you can choose different themes)
import 'primereact/resources/primereact.min.css'; // Core CSS
import 'primeicons/primeicons.css'; // Icons
import { routes } from '@/infrastructure/routing/routes';

export const router = createBrowserRouter(routes);

const appRoot = document.getElementById('root')!;

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <PrimeReactProvider>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
      </QueryClientProvider>
    </PrimeReactProvider>
  </StrictMode>,
);
