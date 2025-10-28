"use server";

import "server-only";

import { cookies } from "next/headers";

import { authCookieNames } from "@helpers/authTokens";
import { verifyAccessToken } from "./jwt";

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
