"use server";

import "server-only";

import { cookies } from "next/headers";

import { authCookieNames } from "@helpers/authTokens";

import { AccessTokenPayload, verifyAccessToken } from "@server/jwt";

export interface ServerAuthState {
  isAuthenticated: boolean;
  token: string | null;
  userId: string | null;
  roles: string[];
  permissions: string[];
  payload: AccessTokenPayload | null;
}

export const getServerAuthState = async (): Promise<ServerAuthState> => {
  const cookieStore = await cookies();
  const token = cookieStore.get(authCookieNames.access)?.value ?? null;

  if (!token) {
    return {
      isAuthenticated: false,
      token: null,
      userId: null,
      roles: [],
      permissions: [],
      payload: null
    };
  }

  const verified = verifyAccessToken(token);

  if (!verified) {
    return {
      isAuthenticated: false,
      token: null,
      userId: null,
      roles: [],
      permissions: [],
      payload: null
    };
  }

  return {
    isAuthenticated: true,
    token,
    userId: verified.userId,
    roles: verified.roles,
    permissions: verified.permissions,
    payload: verified.payload
  };
};
