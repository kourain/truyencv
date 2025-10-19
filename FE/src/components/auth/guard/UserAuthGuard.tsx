"use client";

import { ReactNode } from "react";
import { GuardContent } from "./GuardContent";

interface UserAuthGuardProps {
  children: ReactNode
}

const USER_AUTH_ROUTE_REGEX = [
  /^\/user\/auth\/(login|register|reset-password)/,
  /^\/user\/auth\/verify-email/,
];
const SKIP_USER_ROUTE_REGEX = [
  /^\/user\/comic\/.+/, // skip for SEO comic + Web Crawler Bot
];
const UserAuthGuard = ({ children }: UserAuthGuardProps) => {
  return (
    <GuardContent USER_AUTH_ROUTE_REGEX={USER_AUTH_ROUTE_REGEX} SKIP_USER_ROUTE_REGEX={SKIP_USER_ROUTE_REGEX} routeFor="user">
      {children}
    </GuardContent>
  );
};

export default UserAuthGuard;
