import type { Metadata } from "next";
import { ReactNode } from "react";

import QueryProvider from "@components/providers/QueryProvider";
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
        <link rel="icon" href="/favicon.ico" />
      </head>
      <body
        data-authenticated={authState.isAuthenticated ? "true" : "false"}
        data-user-id={authState.userId ?? ""}
        data-user-roles={authState.roles.join(",")}
        data-user-permissions={authState.permissions.join(",")}
      >
        <QueryProvider>{children}</QueryProvider>
      </body>
    </html>
  );
};

export default RootLayout;
