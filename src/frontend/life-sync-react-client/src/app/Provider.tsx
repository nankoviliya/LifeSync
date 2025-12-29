import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { PrimeReactProvider } from 'primereact/api';
import * as React from 'react';
import { PropsWithChildren, Suspense } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { I18nextProvider } from 'react-i18next';

import i18n from '@/app/translations/i18n';
import { LanguageSync } from '@/app/translations/LanguageSync';
import { MainErrorFallback } from '@/components/errors/MainErrorFallback';
import { SkeletonLoader } from '@/components/loaders/SkeletonLoader';
import { queryConfig } from '@/lib/reactQuery';
import { AuthProvider } from '@/stores/AuthProvider';
import { ThemeProvider } from '@/stores/ThemeProvider';

export const AppProvider = ({ children }: PropsWithChildren) => {
  const [queryClient] = React.useState(
    () =>
      new QueryClient({
        defaultOptions: queryConfig,
      }),
  );

  return (
    <Suspense fallback={<SkeletonLoader />}>
      <ErrorBoundary FallbackComponent={MainErrorFallback}>
        <ThemeProvider>
          <PrimeReactProvider>
            <QueryClientProvider client={queryClient}>
              <I18nextProvider i18n={i18n}>
                <AuthProvider>
                  <LanguageSync>{children}</LanguageSync>
                </AuthProvider>
              </I18nextProvider>
            </QueryClientProvider>
          </PrimeReactProvider>
        </ThemeProvider>
      </ErrorBoundary>
    </Suspense>
  );
};
