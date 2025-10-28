import { UserLoginContent } from "@components/auth/login/user";
import { Suspense } from "react";
const UserLoginPage = () => (
  <Suspense
    fallback={
      <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
        <div className="flex flex-col items-center gap-3 text-center">
          <span className="h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent" />
          <p className="text-sm text-surface-foreground/80">Đang tải trang đăng nhập...</p>
        </div>
      </div>
    }
  >
    <UserLoginContent />
  </Suspense>
);

export default UserLoginPage;
