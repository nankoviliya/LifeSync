import { StrictMode } from 'react';
import '@/infrastructure/translations/i18n';
import './index.scss';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './infrastructure/api/queryClient/queryClient';
import { createRoot } from 'react-dom/client';
import { PrimeReactProvider } from 'primereact/api';
import 'primereact/resources/themes/lara-light-blue/theme.css'; // Theme (you can choose different themes)
import 'primereact/resources/primereact.min.css'; // Core CSS
import 'primeicons/primeicons.css'; // Icons
import { AppRouter } from '@/infrastructure/routing/AppRouter';
import { AuthProvider } from '@/infrastructure/authentication/context/AuthProvider';
import { prefetchAll } from '@/infrastructure/api/prefetch/prefetchAll';
import i18n from '@/infrastructure/translations/i18n';
import { I18nextProvider } from 'react-i18next';
import { ErrorBoundary } from 'react-error-boundary';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';

prefetchAll().catch((err) => {
  console.error('Error prefetching data:', err);
});

const appRoot = document.getElementById('root')!;

const root = createRoot(appRoot);

root.render(
  <StrictMode>
    <ErrorBoundary FallbackComponent={MainErrorFallback}>
      <PrimeReactProvider>
        <QueryClientProvider client={queryClient}>
          <I18nextProvider i18n={i18n}>
            <AuthProvider>
              <AppRouter />
            </AuthProvider>
          </I18nextProvider>
        </QueryClientProvider>
      </PrimeReactProvider>
    </ErrorBoundary>
  </StrictMode>,
);
