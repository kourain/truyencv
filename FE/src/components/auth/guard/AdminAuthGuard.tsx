"use client";

import { ReactNode } from "react";
import { GuardContent } from "./GuardContent";

interface AdminAuthGuardProps {
  children: ReactNode;
  routeFor?: "admin" | "converter";
}

const AUTH_ROUTE_REGEX: Record<"admin" | "converter", RegExp[]> = {
  admin: [/^\/admin\/auth\/(login|reset-password)/],
  converter: [/^\/converter\/auth\/(login|reset-password)/]
};

const AdminAuthGuard = ({ children, routeFor = "admin" }: AdminAuthGuardProps) => {
  const patterns = AUTH_ROUTE_REGEX[routeFor] ?? AUTH_ROUTE_REGEX.admin;
  return (
    <GuardContent USER_AUTH_ROUTE_REGEX={patterns} routeFor={routeFor}>
      {children}
    </GuardContent>
  );
};
export default AdminAuthGuard;
