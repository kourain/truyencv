import type { Metadata } from "next";
import { ReactNode } from "react";

import QueryProvider from "@components/providers/QueryProvider";
import AuthProvider from "@components/providers/AuthProvider";
import ToastProvider from "@components/providers/ToastProvider";

import "./globals.css";
import { redirect } from "next/navigation";
import { headers } from "next/headers";
import { emptyServerAuthState } from "@const/val";

export const metadata: Metadata = {
  title: {
    default: "TruyenCV",
    template: "%s | TruyenCV"
  },
  description: "Nền tảng đọc truyện trực tuyến TruyenCV"
};

const RootLayout = async ({ children }: { children: ReactNode }) => {
  const headersList = await headers();
  const authStateBase64 = headersList.get("x-auth-state");
  let authState = emptyServerAuthState as { userProfile: UserProfileResponse; auth: AuthTokensResponse };
  
  if (authStateBase64) {
    try {
      authState = JSON.parse(Buffer.from(authStateBase64, 'base64').toString('utf-8'));
    } catch (error) {
      console.error('Failed to parse auth state:', error);
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
            </QueryProvider>
          </ToastProvider>
        </AuthProvider>
      </body>
    </html>
  );
};

export default RootLayout;
