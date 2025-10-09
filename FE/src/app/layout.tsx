import type { Metadata } from "next";
import { ReactNode } from "react";

import QueryProvider from "@components/providers/QueryProvider";

import "./globals.css";

export const metadata: Metadata = {
  title: {
    default: "TruyenCV",
    template: "%s | TruyenCV"
  },
  description: "Nền tảng đọc truyện trực tuyến TruyenCV"
};

const RootLayout = ({ children }: { children: ReactNode }) => (
  <html lang="vi">
    <body>
      <QueryProvider>{children}</QueryProvider>
    </body>
  </html>
);

export default RootLayout;
