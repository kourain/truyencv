"use server";

import "server-only";

import { cookies } from "next/headers";

import { authCookieNames } from "@helpers/authTokens";

import { AccessTokenPayload, verifyAccessToken } from "@server/jwt";

export interface ServerAuthState {
  isAuthenticated: boolean;
  token: string | null;
  userId: string | null;
  name: string | null;
  avatar: string | null;
  email: string | null;
  roles: string[];
  permissions: string[];
  payload: AccessTokenPayload | null;
}

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
export const getServerAuthState = async (): Promise<ServerAuthState> => {
  const cookieStore = await cookies();
  const token = cookieStore.get(authCookieNames.access)?.value ?? null;
  return await parseJwtToken(token);
};
