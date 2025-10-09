import { Suspense } from "react";

import { UserSearchContent } from "@components/user/search/UserSearchContent";

const UserSearchPage = () => (
  <Suspense
    fallback={
      <div className="flex min-h-screen items-center justify-center bg-surface text-surface-foreground">
        <div className="flex flex-col items-center gap-3 text-center">
          <span className="h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent" />
          <p className="text-sm text-surface-foreground/80">Đang tải giao diện tìm kiếm...</p>
        </div>
      </div>
    }
  >
    <UserSearchContent />
  </Suspense>
);

export default UserSearchPage;
