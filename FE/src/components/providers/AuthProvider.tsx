"use client";

import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";

import { parseJwtToken, type ServerAuthState } from "@server/auth";

export type AuthContextValue = ServerAuthState & {
  overrideAuthState: (nextState: ServerAuthState) => void;
  updateAuthState: (partialState: Partial<ServerAuthState>) => void;
  updateAuthStateFromAccessToken: (token: string | null) => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  initialState: ServerAuthState;
  children: ReactNode;
}

const AuthProvider = ({ initialState, children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<ServerAuthState>(() => initialState);

  const overrideAuthState = useCallback((nextState: ServerAuthState) => {
    setAuthState(nextState);
  }, []);

  const updateAuthState = useCallback((partialState: Partial<ServerAuthState>) => {
    setAuthState((prev) => ({ ...prev, ...partialState }));
  }, []);

  const updateAuthStateFromAccessToken = async (token: string | null) => {
    const newState = await parseJwtToken(token);
    setAuthState(newState);
  };
  const value = useMemo<AuthContextValue>(
    () => ({
      ...authState,
      overrideAuthState,
      updateAuthState,
      updateAuthStateFromAccessToken
    }),
    [authState, overrideAuthState, updateAuthState, updateAuthStateFromAccessToken]
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
