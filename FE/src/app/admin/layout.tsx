"use client";
import { ReactNode } from "react";

import AdminAuthGuard from "@components/user/auth/guard/AdminAuthGuard";
import AdminSidebar from "@components/admin/shared/Sidebar";
import Footer from "@components/user/shared/Footer";
import { useAuth } from "@hooks/useAuth";
import AdminNavbar from "@components/admin/shared/Navbar";

const AdminLayout = ({ children }: { children: ReactNode }) => {
  const auth = useAuth();
  return (
    <AdminAuthGuard>
      <div className="flex min-h-screen flex-col bg-surface text-surface-foreground">
        {auth.isAuthenticated && <AdminNavbar />}
        <div className="flex">
          {auth.isAuthenticated && <AdminSidebar />}
          <main className="flex flex-col overflow-x-hidden bg-surface px-4 py-8 md:px-6 w-[100%]">
            {children}
            <Footer />
          </main>
        </div>
      </div>
    </AdminAuthGuard>
  );
};

export default AdminLayout;
