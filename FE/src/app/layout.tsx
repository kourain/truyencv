import type { Metadata } from "next";
import { ReactNode } from "react";

import QueryProvider from "@components/providers/QueryProvider";
import AuthProvider from "@components/providers/AuthProvider";
import ToastProvider from "@components/providers/ToastProvider";
import { getServerAuthState } from "@server/auth";

import "./globals.css";

export const metadata: Metadata = {
  title: {
    default: "TruyenCV",
    template: "%s | TruyenCV"
  },
  description: "Nền tảng đọc truyện trực tuyến TruyenCV"
};

const RootLayout = async ({ children }: { children: ReactNode }) => {
  const authState = await getServerAuthState();

  return (
    <html lang="vi">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="icon" href="/favicon.svg" type="image/svg+xml" />
      </head>
      <body>
        <AuthProvider initialState={authState}>
          <ToastProvider>
            <QueryProvider>{children}</QueryProvider>
          </ToastProvider>
        </AuthProvider>
      </body>
    </html>
  );
};

export default RootLayout;
