"use client";

import { ReactNode } from "react";
import { GuardContent } from "./GuardContent";

interface AdminAuthGuardProps {
  children: ReactNode
}

const ADMIN_AUTH_ROUTE_REGEX = [
  /^\/admin\/auth\/(login|reset-password)/
];
const AdminAuthGuard = ({ children }: AdminAuthGuardProps) => {
  return (
    <GuardContent USER_AUTH_ROUTE_REGEX={ADMIN_AUTH_ROUTE_REGEX} routeFor="admin">
      {children}
    </GuardContent>
  );
};
export default AdminAuthGuard;
