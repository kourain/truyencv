"use client";

import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";

import { parseJwtToken, type ServerAuthState } from "@server/auth";

export type AuthContextValue = ServerAuthState & {
  updateAuthStateFromAccessToken: (token: string) => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  initialState: ServerAuthState;
  children: ReactNode;
}

const AuthProvider = ({ initialState, children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<ServerAuthState>(() => initialState);

  const updateAuthStateFromAccessToken = useCallback(async (token: string) => {
    const newState = await parseJwtToken(token);
    setAuthState(newState);
  }, [authState, setAuthState]);
  const value = useMemo<AuthContextValue>(
    () => ({
      ...authState,
      updateAuthStateFromAccessToken
    }),
    [authState]
  );
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuthContext = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuthContext must be used within an AuthProvider");
  }

  return context;
};

export default AuthProvider;
