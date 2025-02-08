import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { PrimeReactProvider } from 'primereact/api';
import * as React from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { I18nextProvider } from 'react-i18next';

import i18n from '@/app/translations/i18n';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';
import { queryConfig } from '@/lib/reactQuery';
import { AuthProvider } from '@/stores/AuthProvider';

type AppProviderProps = {
  children: React.ReactNode;
};

export const AppProvider = ({ children }: AppProviderProps) => {
  const [queryClient] = React.useState(
    () =>
      new QueryClient({
        defaultOptions: queryConfig,
      }),
  );

  return (
    <ErrorBoundary FallbackComponent={MainErrorFallback}>
      <PrimeReactProvider>
        <QueryClientProvider client={queryClient}>
          <I18nextProvider i18n={i18n}>
            <AuthProvider>{children}</AuthProvider>
          </I18nextProvider>
        </QueryClientProvider>
      </PrimeReactProvider>
    </ErrorBoundary>
  );
};
