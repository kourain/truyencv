import type { Metadata } from "next";
import { ReactNode } from "react";

import UserAuthGuard from "@components/auth/guard/UserAuthGuard";
import Footer from "@components/user/shared/Footer";
import UserNavbar from "@components/user/shared/Navbar";

export const metadata: Metadata = {
  title: "Bảng điều khiển User"
};

const UserLayout = async ({ children }: { children: ReactNode }) => {
  return (
    <UserAuthGuard>
      <div className="relative flex min-h-screen flex-col bg-gradient-to-br from-surface via-surface-muted to-surface">
        <UserNavbar />
        {children}
        <Footer />
      </div>
    </UserAuthGuard>
  );
};

export default UserLayout;
