import { AppProvider } from '@/app/Provider';
import { AppRouter } from '@/app/routing/AppRouter';

export const App = () => {
  return (
    <AppProvider>
      <AppRouter />
    </AppProvider>
  );
};
