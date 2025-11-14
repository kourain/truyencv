import type { NextConfig } from "next";

const {
  BACKEND_URL = "",
  COMIC_CDN_URL = "",
  JWT_SECRET = "",
  FE_USER = "",
  FE_ADMIN = "",
} = process.env;

const nextConfig: NextConfig = {
  reactStrictMode: true,
  typedRoutes: true,
  env: {
    NEXT_PUBLIC_BACKEND_URL: BACKEND_URL,
    NEXT_PUBLIC_CDN_URL: COMIC_CDN_URL,
    NEXT_PUBLIC_JWT_SECRET: JWT_SECRET,
    NEXT_PUBLIC_FE_USER: FE_USER,
    NEXT_PUBLIC_FE_ADMIN: FE_ADMIN,
  },
  redirects: async () => {
    return [];
  },
  rewrites: async () => {
    return [
      {
        source: "/user/profile/info",
        destination: "/user/profile?section=info",
      },
      {
        source: "/user/profile/history_coin",
        destination: "/user/profile?section=history_coin",
      },
      {
        source: "/user/profile/history_deposit",
        destination: "/user/profile?section=history_deposit",
      },
      {
        source: "/user/profile/history_ticket",
        destination: "/user/profile?section=history_ticket",
      },
      {
        source: "/user/profile/history_read",
        destination: "/user/profile?section=history_read",
      },
      {
        source: "/user/profile/history_comment",
        destination: "/user/profile?section=history_comment",
      },
      {
        source: "/user/profile/security_email",
        destination: "/user/profile?section=security_email",
      },
      {
        source: "/user/profile/security_password",
        destination: "/user/profile?section=security_password",
      },
    ];
  },
  allowedDevOrigins: [
    ...FE_ADMIN.split(","),
    ...FE_USER.split(",")
  ],
  productionBrowserSourceMaps : false,
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "*",
        port: "",
        pathname: "/**",
      },
    ],
  },
};

export default nextConfig;
