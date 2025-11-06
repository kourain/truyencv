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
    return [
      // {
      //   source: "/:path*",
      //   has: [
      //     {
      //       type: "host",
      //       value: FE_ADMIN,
      //     },
      //   ],
      //   destination: "/admin/:path*",
      //   permanent: true
      // }, {
      //   source: "/:path*",
      //   has: [
      //     {
      //       type: "host",
      //       value: FE_USER,
      //     },
      //   ],
      //   destination: "/user/:path*",
      //   permanent: true
      // }
    ];
  },
  allowedDevOrigins: [
    ...FE_ADMIN.split(","),
    ...FE_USER.split(",")
  ],
  productionBrowserSourceMaps : false,
};

export default nextConfig;
