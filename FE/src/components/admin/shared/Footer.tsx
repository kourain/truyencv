import { usePathname } from "next/navigation";

export default function Footer() {
  const pathname = usePathname();
  if (pathname.match(/login|register|reset-password|verify-email/)) {
    return null;
  }
  return (
    <footer className="border-t border-surface-muted/60 bg-surface py-6">
      <div className="mx-auto max-w-6xl px-6">
        <p className="text-sm text-surface-foreground/60">
          © 2025 TruyenCV. All rights reserved.
        </p>
      </div>
    </footer>
  );
}
