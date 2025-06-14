import { useQueryClient } from '@tanstack/react-query';
import { jwtDecode } from 'jwt-decode';
import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from 'react';

interface AuthContextType {
  token: string | null;
  login: (token: string) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

export interface IAuthProviderProps {
  children: React.ReactNode;
}

export const AuthProvider = ({ children }: IAuthProviderProps) => {
  const queryClient = useQueryClient();

  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token'),
  );

  const login = useCallback((token: string) => {
    setToken(token);
    localStorage.setItem('token', token);
    queryClient.clear();
  }, []);

  const logout = useCallback(() => {
    setToken(null);
    localStorage.removeItem('token');
  }, []);

  const isAuthenticated = Boolean(token);

  useEffect(() => {
    if (token) {
      const decodedToken: any = jwtDecode(token);

      if (decodedToken.exp * 1000 < Date.now()) {
        logout();
      }
    }
  }, [token, logout]);

  // React Context Provider values should have stable identities typescript:S6481
  const contextValue = useMemo(
    () => ({
      token,
      login,
      logout,
      isAuthenticated,
    }),
    [token, login, logout, isAuthenticated],
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};
