import { UserRole } from "@const/enum/role";
import {
  clearAuthTokens,
  getAccessTokenPayload,
  getRoleFromJWT
} from "@helpers/authTokens";
import { useAuth } from "@hooks/useAuth";
import { refreshTokens } from "@services/auth.service";
import type { Route } from "next";
import { usePathname, useRouter } from "next/navigation";
import { ReactNode, useEffect, useMemo, useState } from "react";

const routeRequiredRoles: Record<string, UserRole[]> = {
  admin: [UserRole.Admin],
  user: [UserRole.User]
};

export const GuardContent = ({ children, USER_AUTH_ROUTE_REGEX, SKIP_USER_ROUTE_REGEX, routeFor }: { children: ReactNode, USER_AUTH_ROUTE_REGEX: RegExp[], SKIP_USER_ROUTE_REGEX: RegExp[], routeFor: string }) => {
  const router = useRouter();
  const pathname = usePathname();
  const [isReady, setIsReady] = useState(false);
  const authState = useAuth();
  const requiredRoles = useMemo(() => routeRequiredRoles[routeFor] ?? [UserRole.User], [routeFor]);
  const [isEmptyPage, setIsEmptyPage] = useState(false);
  const hasRequiredRole = (roles?: string[]) => {
    if (!roles?.length) {
      return false;
    }
    return roles?.some((role) => requiredRoles.includes(role as UserRole));
  };

  useEffect(() => {
    let isMounted = true;
    const finish = () => {
      if (isMounted) {
        setIsReady(true);
      }
    };

    const ensureUserSession = async () => {
      const isSessionValid = hasRequiredRole(authState.roles) && authState.isAuthenticated;

      if (isSessionValid) {
        finish();
        return;
      }

      const attemptRefresh = async () => {
        try {
          const tokens = await refreshTokens();
          await authState.updateAuthStateFromAccessToken(tokens.access_token);
          const refreshedPayload = getAccessTokenPayload();
          const refreshedRoles = refreshedPayload ? getRoleFromJWT(refreshedPayload) : [];
          return tokens.access_token?.length > 0 && hasRequiredRole(refreshedRoles);
        } catch (error) {
          console.error("[AuthGuard] Không thể làm mới phiên người dùng", error);
          return false;
        }
      };

      if (USER_AUTH_ROUTE_REGEX.some((regex) => regex.test(pathname ?? ""))) {
        if (isSessionValid) {
          router.replace(`/${routeFor}` as Route);
          finish();
          return;
        }

        const refreshed = await attemptRefresh();
        if (refreshed) {
          router.replace(`/${routeFor}` as Route);
        } else {
          await clearAuthTokens();
        }
        finish();
        return;
      }

      const refreshed = await attemptRefresh();
      if (refreshed) {
        finish();
      } else {
        await clearAuthTokens();
        router.replace(`/${routeFor}/auth/login` as Route);
        finish();
      }
    };

    if (SKIP_USER_ROUTE_REGEX.some((regex) => regex.test(pathname ?? ""))) {
      setIsEmptyPage(true);
      finish();
    }
    else {
      ensureUserSession();
    }

    return () => {
      isMounted = false;
    };
  });

  return (
    <>
      {isEmptyPage === true ? null :
        isReady ? (
          children
        ) : (
          <div className="flex min-h-[60vh] items-center justify-center">
            <div className="flex flex-col items-center gap-3 rounded-2xl border border-surface-muted bg-surface/80 px-6 py-8 text-sm text-surface-foreground/70">
              <div className="h-10 w-10 animate-spin rounded-full border-2 border-primary border-t-transparent" />
              <p>Đang xác thực quyền truy cập...</p>
            </div>
          </div>
        )}
    </>
  );
}