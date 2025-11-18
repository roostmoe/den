import { createContext, useCallback, useContext, useEffect, useState, type PropsWithChildren } from "react";
import { postV1AuthRefresh } from "./client";
import { client } from "./client/client.gen";

type AuthTokens = {
  accessToken: string;
  refreshToken: string;
};

type AuthContext = {
  tokens: AuthTokens | null;
  refresh: () => Promise<void>;
  login: (tokens: AuthTokens) => void;
  logout: () => void;
  isAuthenticated: boolean;
};

export const authContext = createContext<AuthContext | undefined>(undefined);

const ACCESS_TOKEN_KEY = 'den_access_token';
const REFRESH_TOKEN_KEY = 'den_refresh_token';

export const AuthProvider = ({ children }: PropsWithChildren) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(() => {
    const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    return accessToken && refreshToken ? { accessToken, refreshToken } : null;
  });

  useEffect(() => {
    if (tokens) {
      localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
      client.setConfig({ auth: tokens.accessToken });
    } else {
      localStorage.removeItem(ACCESS_TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
      client.setConfig({ auth: undefined });
    }
  }, [tokens]);

  const login = useCallback((newTokens: AuthTokens) => {
    setTokens(newTokens);
  }, []);

  const logout = useCallback(() => {
    setTokens(null);
  }, []);

  const refresh = useCallback(async () => {
    const currentRefreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!currentRefreshToken) return;
    
    try {
      const { data } = await postV1AuthRefresh({
        body: { refreshToken: currentRefreshToken },
      });

      setTokens({
        accessToken: data?.accessToken!!,
        refreshToken: currentRefreshToken,
      });
    } catch (err) {
      // Refresh failed, log out user.
      logout();
    }
  }, [logout]);

  // Refresh on mount if we have a refresh token
  useEffect(() => {
    if (tokens?.refreshToken) {
      refresh();
    }
  }, []);

  // Periodic refresh
  useEffect(() => {
    if (!tokens?.refreshToken) return;

    const refreshInterval = setInterval(refresh, 2.5 * 60 * 1000 /* Every 2.5 minutes (a half token life-time) */);

    return () => clearInterval(refreshInterval);
  }, [tokens?.refreshToken, refresh]);

  return (
    <authContext.Provider value={{
      tokens,
      login,
      logout,
      refresh,
      isAuthenticated: !!tokens,
    }}>
      {children}
    </authContext.Provider>
  )
};

export const useAuth = () => {
  const ctx = useContext(authContext);
  if (!ctx) {
    throw new Error('useAuth() must be called within an <AuthProvider />');
  }
  return ctx;
};
