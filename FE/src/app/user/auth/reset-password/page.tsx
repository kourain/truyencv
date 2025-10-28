import { UserResetPasswordContent } from "@components/user/auth/reset/user";
import { Suspense } from "react";

const UserResetPasswordPage = () => (
  <Suspense
    fallback={
      <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
        <div className="flex flex-col items-center gap-3 text-center">
          <span className="h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent" />
          <p className="text-sm text-surface-foreground/80">Đang tải trang đặt lại mật khẩu...</p>
        </div>
      </div>
    }
  >
    <UserResetPasswordContent />
  </Suspense>
);

export default UserResetPasswordPage;
