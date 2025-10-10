import type { ComponentType } from "react";

export type MetricCardConfig = {
  label: string;
  value: string;
  description: string;
  icon: ComponentType<{ className?: string }>;
};

export type OverviewStats = {
  totalComics: number;
  continuing: number;
  completed: number;
  totalUsers: number;
  newUsers: number;
  categoriesCount: number;
  totalChapters: number;
  totalComments: number;
  totalBookmarks: number;
  activeAdmins: number;
};
