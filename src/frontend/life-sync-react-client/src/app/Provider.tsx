import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { PrimeReactProvider } from 'primereact/api';
import * as React from 'react';
import { Suspense } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { I18nextProvider } from 'react-i18next';

import { AuthGate } from '@/app/AuthGate';
import i18n from '@/app/translations/i18n';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';
import { queryConfig } from '@/lib/reactQuery';
import { AuthProvider } from '@/stores/AuthProvider';
import { ThemeProvider } from '@/stores/ThemeProvider';

type AppProviderProps = {
  children: React.ReactNode;
};

export const AppLoadingFallback = () => {
  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        fontSize: '1.2rem',
        color: '#666',
      }}
    >
      Loading...
    </div>
  );
};

export const AppProvider = ({ children }: AppProviderProps) => {
  const [queryClient] = React.useState(
    () =>
      new QueryClient({
        defaultOptions: queryConfig,
      }),
  );

  return (
    <Suspense fallback={<AppLoadingFallback />}>
      <ErrorBoundary FallbackComponent={MainErrorFallback}>
        <ThemeProvider>
          <PrimeReactProvider>
            <QueryClientProvider client={queryClient}>
              <I18nextProvider i18n={i18n}>
                <AuthProvider>
                  <AuthGate>{children}</AuthGate>
                </AuthProvider>
              </I18nextProvider>
            </QueryClientProvider>
          </PrimeReactProvider>
        </ThemeProvider>
      </ErrorBoundary>
    </Suspense>
  );
};
