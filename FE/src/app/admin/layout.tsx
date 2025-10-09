import type { Metadata } from "next";
import { ReactNode } from "react";

import AdminAuthGuard from "@components/auth/guard/AdminAuthGuard";

export const metadata: Metadata = {
  title: "Bảng điều khiển Admin"
};

const AdminLayout = ({ children }: { children: ReactNode }) => {
  return (
    <AdminAuthGuard>{children}</AdminAuthGuard>
  );
};

export default AdminLayout;
