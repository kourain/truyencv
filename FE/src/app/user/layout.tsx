import type { Metadata } from "next";
import { ReactNode } from "react";

import UserAuthGuard from "@components/auth/guard/UserAuthGuard";

export const metadata: Metadata = {
  title: "Bảng điều khiển User"
};

const UserLayout = ({ children }: { children: ReactNode }) => {
  return (
    <UserAuthGuard>{children}</UserAuthGuard>
  );
};

export default UserLayout;
