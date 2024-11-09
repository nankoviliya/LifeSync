import { StrictMode } from 'react';
import './index.css';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './infrastructure/api/queryClient/queryClient';
import { createRoot } from 'react-dom/client';
import { PrimeReactProvider } from 'primereact/api';
import 'primereact/resources/themes/lara-light-blue/theme.css'; // Theme (you can choose different themes)
import 'primereact/resources/primereact.min.css'; // Core CSS
import 'primeicons/primeicons.css'; // Icons
import { AppRouter } from '@/infrastructure/routing/AppRouter';
import { AuthProvider } from '@/infrastructure/authentication/context/AuthProvider';

const appRoot = document.getElementById('root')!;

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <PrimeReactProvider>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <AppRouter />
        </AuthProvider>
      </QueryClientProvider>
    </PrimeReactProvider>
  </StrictMode>,
);
