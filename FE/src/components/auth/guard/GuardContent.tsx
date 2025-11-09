import { UserRole } from "@const/enum/role";
import {
  clearAuthTokens,
} from "@helpers/authTokens";
import { useAuth } from "@hooks/useAuth";
import type { Route } from "next";
import { usePathname, useRouter } from "next/navigation";
import { ReactNode, Suspense, useEffect, useMemo, useState } from "react";

const routeRequiredRoles: Record<string, UserRole[]> = {
  "admin": [UserRole.Admin],
  "user": [UserRole.User]
};

export const GuardContent = ({ children, USER_AUTH_ROUTE_REGEX, routeFor }: { children: ReactNode, USER_AUTH_ROUTE_REGEX: RegExp[], routeFor: string }) => {
  const pathname = usePathname();
  const router = useRouter();
  const [isReady, setIsReady] = useState(false);
  const authState = useAuth();
  const requiredRoles = useMemo(() => routeRequiredRoles[routeFor], [routeFor]);
  const hasRequiredRole = (roles?: string[]) => {
    if (!roles?.length) {
      return false;
    }
    return roles?.some((role) => requiredRoles.includes(role as UserRole));
  };
  useEffect(() => {
    const isSessionValid = hasRequiredRole(authState.userProfile.roles) && authState.isAuthenticated;
    const ensureUserSession = async (): Promise<void> => {
      if (isSessionValid === false) {
        console.log(authState)
        console.log("GuardContent: Invalid session, redirecting to login...");
        await clearAuthTokens();
        router.replace(`/${routeFor}/auth/login` as Route);
      }
    };
    if (USER_AUTH_ROUTE_REGEX.some((regex) => regex.test(pathname))) {
      if (isSessionValid) {
        router.replace(`/${routeFor}` as Route);
      }
    } else {
      ensureUserSession();
    }
    setIsReady(true);
  }, [pathname]);

  return (
    <>
      <Suspense fallback={
        <div className="flex min-h-[60vh] items-center justify-center">
          <div className="flex flex-col items-center gap-3 rounded-2xl border border-surface-muted bg-surface/80 px-6 py-8 text-sm text-surface-foreground/70">
            <div className="h-10 w-10 animate-spin rounded-full border-2 border-primary border-t-transparent" />
            <p>Đang xác thực quyền truy cập...</p>
          </div>
        </div>
      }>
        {isReady &&
          children
        }
      </Suspense>
    </>
  );
}