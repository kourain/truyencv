"use client";

import { createContext,  useContext, useMemo, useState, type ReactNode } from "react";

import { parseJwtToken, type ServerAuthState } from "@server/auth";

export type AuthContextValue = ServerAuthState & {
  overrideAuthState: (nextState: ServerAuthState) => void;
  updateAuthStateFromAccessToken: (token: string) => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  initialState: ServerAuthState;
  children: ReactNode;
}

const AuthProvider = ({ initialState, children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<ServerAuthState>(initialState);

  const overrideAuthState = (nextState: ServerAuthState) => {
    setAuthState(nextState);
  };

  const updateAuthStateFromAccessToken = async (token: string) => {
    const newState = await parseJwtToken(token);
    setAuthState(newState);
  };
  const value = {
    ...authState,
    overrideAuthState,
    updateAuthStateFromAccessToken
  };
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
