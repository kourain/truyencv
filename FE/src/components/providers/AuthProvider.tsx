"use client";

import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";


export type AuthContextValue = ServerAuthState & {
  updateAuthState: (newState: ServerAuthState) => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  initialState: ServerAuthState;
  children: ReactNode;
}

const AuthProvider = ({ initialState, children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<ServerAuthState>(() => initialState);

  const updateAuthState = useCallback((newState: ServerAuthState) => {
    setAuthState(newState);
  }, [setAuthState]);
  const value = useMemo<AuthContextValue>(
    () => ({
      ...authState,
      updateAuthState
    }),
    [authState, updateAuthState]
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
