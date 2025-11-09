"use client";

import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";

export type AuthContextValue = AuthTokensResponse & {
  isAuthenticated: boolean;
  updateAuthState: (newState: AuthTokensResponse) => void;
  userProfile: UserProfileResponse;
  updateUserProfile: (newProfile: UserProfileResponse) => void;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  initialState: { auth: AuthTokensResponse, userProfile: UserProfileResponse };
  children: ReactNode;
}

const AuthProvider = ({ initialState, children }: AuthProviderProps) => {
  const [authState, setAuthState] = useState<AuthTokensResponse>(() => initialState.auth);
  const [userProfile, setUserProfile] = useState<UserProfileResponse>(() => initialState.userProfile);

  const updateAuthState = useCallback((newState: AuthTokensResponse) => {
    setAuthState(newState);
  }, [setAuthState]);
  const updateUserProfile = useCallback((newProfile: UserProfileResponse) => {
    setUserProfile(newProfile);
  }, [setUserProfile]);
  const value = useMemo<AuthContextValue>(
    () => ({
      ...authState,
      updateAuthState,
      userProfile,
      updateUserProfile,
      isAuthenticated: userProfile.id?.length > 5 || false
    }),
    [authState, updateAuthState, userProfile, updateUserProfile]
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
