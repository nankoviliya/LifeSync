import { StrictMode } from 'react';
import './index.css';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { routes } from '@infrastructure/routing/routes';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './infrastructure/api/queryClient/queryClient';
import { createRoot } from 'react-dom/client';

export const router = createBrowserRouter(routes);

const appRoot = document.getElementById('root')!;

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>,
);
