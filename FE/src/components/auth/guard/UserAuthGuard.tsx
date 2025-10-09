"use client";

import { clearAuthTokens, hasValidTokens, tokenHasRole } from "@helpers/authTokens";
import type { Route } from "next";
import { usePathname, useRouter } from "next/navigation";
import { ReactNode, useEffect, useState } from "react";

interface UserAuthGuardProps {
    children: ReactNode
}

const UserAuthGuard = ({ children }: UserAuthGuardProps) => {
    const router = useRouter();
    const pathname = usePathname();
    const [isReady, setIsReady] = useState(false);
    const isLogin = pathname.includes("/login");

    useEffect(() => {
        if (isLogin) {
            setIsReady(true);
            return;
        }

        const ensureAuth = () => {
            if (!hasValidTokens() || !tokenHasRole("User")) {
                clearAuthTokens();
                router.replace("/user/login" as Route);
                return;
            }

            setIsReady(true);
        };

        ensureAuth();
    }, [isLogin, router]);

    if (!isReady) {
        return (
            <div className="flex min-h-[60vh] items-center justify-center">
                <div className="flex flex-col items-center gap-3 rounded-2xl border border-surface-muted bg-surface/80 px-6 py-8 text-sm text-surface-foreground/70">
                    <div className="h-10 w-10 animate-spin rounded-full border-2 border-primary border-t-transparent" />
                    <p>Đang xác thực quyền truy cập...</p>
                </div>
            </div>
        );
    }

    return <>{children}</>;
};

export default UserAuthGuard;
