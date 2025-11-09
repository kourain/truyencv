"use server";

import "server-only";

import { verifyAccessToken } from "./jwt";
import { fetchUserProfile } from "@services/user/profile.service";

export const parseJwtToken = async (token: string | null): Promise<ServerAuthState> => {
  if (!token) {
    return {
      isAuthenticated: false,
      token: null,
      userId: null,
      name: null,
      avatar: null,
      email: null,
      roles: [],
      permissions: [],
      payload: null
    };
  }

  const verified = await verifyAccessToken(token);

  if (!verified) {
    return {
      isAuthenticated: false,
      token: null,
      userId: null,
      name: null,
      avatar: null,
      email: null,
      roles: [],
      permissions: [],
      payload: null
    };
  }

  return {
    isAuthenticated: true,
    token,
    userId: verified.userId,
    name: verified.name,
    avatar: verified.avatar,
    email: verified.email,
    roles: verified.roles,
    permissions: verified.permissions,
    payload: verified.payload
  };
}
export const getServerAuthState = async (): Promise<{ userProfile: UserProfileResponse, auth: AuthTokensResponse }> => {
  try {
    const token = await fetchUserProfile();
    return {
      auth: {
        ...await parseJwtToken(token.access_token),
        access_token: token.access_token,
        access_token_minutes: token.access_token_minutes,
        refresh_token: token.refresh_token,
        refresh_token_days: token.refresh_token_days
      },
      userProfile: token
    };
  } catch (error) {
    console.error("Error fetching server auth state:", error);
    return {
      auth: {
        access_token: "",
        access_token_minutes: 0,
        refresh_token: "",
        refresh_token_days: 0
      },
      userProfile: {} as UserProfileResponse
    };
  }
};
