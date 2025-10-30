"use server";

import "server-only";

import jwt from "jsonwebtoken";

const JWT_SECRET = process.env.JWT_SECRET;

type ClaimValue = string | string[] | undefined;


const normalizeClaim = (value: ClaimValue): string[] => {
  if (!value) {
    return [];
  }

  if (Array.isArray(value)) {
    return value.map((item) => item?.toString() ?? "").filter(Boolean);
  }

  if (typeof value === "string") {
    return [value];
  }

  return [];
};

export const verifyAccessToken = async (token: string): Promise<VerifiedAccessToken | null> => {
  if (!token) {
    return null;
  }

  if (!JWT_SECRET) {
    console.error("[AUTH] JWT_SECRET is not configured on the server environment");
    return null;
  }

  try {
    const decoded = jwt.verify(token, JWT_SECRET) as JWT;
    const roles = normalizeClaim(
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? decoded.role
    );
    const permissions = normalizeClaim(decoded.permissions);

    return {
      userId: decoded.sub ?? null,
      name: decoded.name ?? null,
      avatar: decoded.avatar ?? null,
      email: decoded.email ?? null,
      roles,
      permissions,
      payload: decoded
    };
  } catch (error) {
    console.error("[AUTH] Failed to verify access token", error);
    return null;
  }
};
