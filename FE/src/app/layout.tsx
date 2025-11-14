import type { Metadata } from "next";
import { ReactNode } from "react";

import QueryProvider from "@components/providers/QueryProvider";
import AuthProvider from "@components/providers/AuthProvider";
import ToastProvider from "@components/providers/ToastProvider";
import { getServerAuthState } from "@server/auth";
import Footer from "@components/layout/Footer";

import "./globals.css";
import { redirect } from "next/navigation";
import { headers } from "next/headers";

export const metadata: Metadata = {
  title: {
    default: "TruyenCV",
    template: "%s | TruyenCV"
  },
  description: "Nền tảng đọc truyện trực tuyến TruyenCV"
};

const RootLayout = async ({ children }: { children: ReactNode }) => {
  let authState = {} as { userProfile: UserProfileResponse; auth: AuthTokensResponse };
  const headersList = await headers();
  const header_url = headersList.get('x-url') || "";
  if (!header_url.includes("/auth/") || !header_url.includes("/privacy-policy") || !header_url.includes("/terms-of-service")) {
    authState = await getServerAuthState();
    if (authState.userProfile.id === "-1") {
      if (header_url.startsWith("/admin")) {
        redirect("/admin/auth/login");
      }
      if (header_url.startsWith("/user")) {
        redirect("/user/auth/login");
      }
    }
  }
  return (
    <html lang="vi">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="icon" href="/favicon.svg" type="image/svg+xml" />
      </head>
      <body className="bg-white">
        <AuthProvider initialState={authState}>
          <ToastProvider>
            <QueryProvider>
              {children}
              <Footer />
            </QueryProvider>
          </ToastProvider>
        </AuthProvider>
      </body>
    </html>
  );
};

export default RootLayout;
