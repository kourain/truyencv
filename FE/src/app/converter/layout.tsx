"use client";
import { ReactNode } from "react";

import AdminAuthGuard from "@components/auth/guard/AdminAuthGuard";
import AdminSidebar from "@components/admin/shared/Sidebar";
import Footer from "@components/user/shared/Footer";
import AdminNavbar from "@components/admin/shared/Navbar";

const ConverterLayout = ({ children }: { children: ReactNode }) => {
  return (
    <AdminAuthGuard routeFor="converter">
      <div className="flex min-h-screen flex-col bg-surface text-surface-foreground">
        <AdminNavbar variant="converter" />
        <div className="flex">
          <AdminSidebar variant="converter" />
          <main className="flex flex-col overflow-x-hidden bg-surface px-4 py-8 md:px-6 w-[100%]">
            {children}
            <Footer />
          </main>
        </div>
      </div>
    </AdminAuthGuard>
  );
};

export default ConverterLayout;
