import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: ["class"],
  content: ["./src/app/**/*.{ts,tsx}", "./src/components/**/*.{ts,tsx}", "./src/hooks/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#8B5CF6",
          foreground: "#FFFFFF",
          subtle: "#C4B5FD"
        },
        secondary: {
          DEFAULT: "#F97316",
          foreground: "#FFFFFF"
        },
        surface: {
          DEFAULT: "#0F172A",
          foreground: "#E2E8F0",
          muted: "#1E293B"
        }
      },
      boxShadow: {
        glow: "0 20px 40px -15px rgba(139, 92, 246, 0.45)"
      }
    }
  },
  plugins: []
};

export default config;
