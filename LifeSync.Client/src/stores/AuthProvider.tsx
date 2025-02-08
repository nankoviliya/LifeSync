import { jwtDecode } from 'jwt-decode';
import { createContext, useEffect, useState } from 'react';

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
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token'),
  );

  const login = (token: string) => {
    setToken(token);
    localStorage.setItem('token', token);
  };

  const logout = () => {
    setToken(null);
    localStorage.removeItem('token');
  };

  const isAuthenticated = Boolean(token);

  useEffect(() => {
    if (token) {
      const decodedToken: any = jwtDecode(token);

      if (decodedToken.exp * 1000 < Date.now()) {
        logout();
      }
    }
  }, [token]);

  return (
    <AuthContext.Provider value={{ token, login, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
};
