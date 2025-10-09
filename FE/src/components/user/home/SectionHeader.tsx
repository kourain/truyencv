"use client";

import type { ReactNode } from "react";

interface SectionHeaderProps {
  icon: ReactNode;
  title: string;
  description?: string;
}

const SectionHeader = ({ icon, title, description }: SectionHeaderProps) => (
  <div className="flex items-center justify-between gap-4">
    <div className="flex items-center gap-3">
      <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-primary/10">{icon}</div>
      <div className="flex flex-col">
        <h2 className="text-lg font-semibold text-primary-foreground">{title}</h2>
        {description && <p className="text-xs text-surface-foreground/60">{description}</p>}
      </div>
    </div>
  </div>
);

export default SectionHeader;
