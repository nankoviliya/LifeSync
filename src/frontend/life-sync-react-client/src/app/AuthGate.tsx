import { useAuth } from '@/hooks/useAuthentication';
import { AuthLoader } from '@/lib/auth/AuthLoader';

export const AuthGate = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <>{children}</>;
  }

  return <AuthLoader>{children}</AuthLoader>;
};
