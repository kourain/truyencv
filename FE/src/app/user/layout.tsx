import type { Metadata } from "next";
import { ReactNode } from "react";

import UserAuthGuard from "@components/auth/guard/UserAuthGuard";
import Footer from "@components/user/shared/Footer";
import UserNavbar from "@components/user/shared/Navbar";

export const metadata: Metadata = {
  title: {
    default: "TruyenCV - Kênh đọc truyện online miễn phí",
    template: "%s | TruyenCV",
  },
  description: "Nền tảng đọc truyện trực tuyến TruyenCV",
  keywords: ["truyện", "đọc truyện", "truyện online"],
  openGraph: {
    siteName: "TruyenCV",
    locale: "vi_VN",
    type: "website",
  },
};

const UserLayout = async ({ children }: { children: ReactNode }) => {
  return (
    <UserAuthGuard>
      <div className="relative flex min-h-screen flex-col bg-surface">
        <UserNavbar />
        {children}
        <Footer />
      </div>
    </UserAuthGuard>
  );
};

export default UserLayout;
