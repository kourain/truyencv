import type { NextConfig } from "next";

const {
  BACKEND_URL = "",
  COMIC_CDN_URL = "",
} = process.env;

const nextConfig: NextConfig = {
  reactStrictMode: true,
  typedRoutes: true,
  env: {
    NEXT_PUBLIC_BACKEND_URL: BACKEND_URL,
    NEXT_PUBLIC_CDN_URL: COMIC_CDN_URL,
  },
  redirects: async () => {
    return [
      {
        source: "/admin",
        destination: "/admin/dashboard",
        permanent: true
      }
    ];
  }
};

export default nextConfig;
