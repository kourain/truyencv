"use client";

import Image from "next/image";

interface AdsBannerProps {
  advertisement?: ComicDetailAdvertisement;
  variant?: "primary" | "secondary" | "tertiary";
  isLoading?: boolean;
}

const variantStyles: Record<"primary" | "secondary" | "tertiary", string> = {
  primary: "border-primary/40 bg-primary/5",
  secondary: "border-secondary/40 bg-secondary/5",
  tertiary: "border-surface-muted/50 bg-surface-muted/40",
};

const AdsBanner = ({ advertisement, variant = "primary", isLoading = false }: AdsBannerProps) => {
  return null;
  // if (isLoading) {
  //   return <div className="h-48 w-full animate-pulse rounded-xl border border-surface-muted/50 bg-surface-muted/40" />;
  // }

  // if (!advertisement) {
  //   return null;
  // }

  // return (
  //   <a
  //     href={advertisement.href}
  //     target="_blank"
  //     rel="noopener noreferrer"
  //     className={`relative block overflow-hidden rounded-xl border p-4 shadow-lg transition hover:-translate-y-0.5 hover:shadow-xl ${variantStyles[variant]}`}
  //   >
  //     <div className="relative flex flex-col gap-4 overflow-hidden rounded-lg border border-white/5 bg-black/20 p-4 backdrop-blur">
  //       <div className="flex flex-col gap-1 text-primary-foreground">
  //         <span className="text-xs uppercase tracking-[0.35em] text-primary-foreground/70">Quảng cáo</span>
  //         <h3 className="text-2xl font-semibold">{advertisement.label}</h3>
  //         {advertisement.description && (
  //           <p className="max-w-3xl text-sm text-primary-foreground/80">{advertisement.description}</p>
  //         )}
  //       </div>
  //       <div className="relative h-40 w-full overflow-hidden rounded-lg border border-white/10">
  //         <Image
  //           src={advertisement.image_url}
  //           alt={advertisement.label}
  //           fill
  //           sizes="(min-width: 768px) 900px, 100vw"
  //           className="object-cover"
  //           priority
  //           unoptimized
  //         />
  //       </div>
  //     </div>
  //   </a>
  // );
};

export default AdsBanner;
