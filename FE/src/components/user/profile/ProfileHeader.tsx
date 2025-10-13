"use client";

import Image from "next/image";
import { BadgeCheck, Ban } from "lucide-react";

interface ProfileHeaderProps {
  profile: UserProfileResponse;
}

const ProfileHeader = ({ profile }: ProfileHeaderProps) => {
  const isVerified = Boolean(profile.email_verified_at);
  const isBanned = profile.is_banned;
  const avatarSrc = profile.avatar?.trim() || null;
  const formatDate = (value: string | null) => {
    if (!value) {
      return "Chưa cập nhật";
    }

    return new Date(value).toLocaleString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <section className="flex flex-col gap-6 rounded-3xl border border-surface-muted bg-surface/80 p-6 shadow-lg lg:flex-row lg:items-center lg:gap-8 lg:p-8">
      <div className="relative flex h-32 w-32 items-center justify-center overflow-hidden rounded-2xl border border-surface-muted/80 bg-surface-muted/40 shadow-inner">
        {avatarSrc ? (
          <Image
            src={avatarSrc}
            alt={profile.name}
            fill
            sizes="128px"
            className="object-cover"
            unoptimized
          />
        ) : (
          <span className="text-4xl font-semibold text-primary">
            {profile.name.slice(0, 1).toUpperCase()}
          </span>
        )}
      </div>

      <div className="flex flex-1 flex-col gap-4">
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-2xl font-semibold text-primary-foreground">{profile.name}</h1>
          {isVerified && (
            <span className="inline-flex items-center gap-1 rounded-full border border-emerald-500/60 bg-emerald-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-emerald-200">
              <BadgeCheck className="h-3.5 w-3.5" />
              Email đã xác thực
            </span>
          )}
          {isBanned && (
            <span className="inline-flex items-center gap-1 rounded-full border border-red-500/60 bg-red-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-red-200">
              <Ban className="h-3.5 w-3.5" />
              Đang bị khóa
            </span>
          )}
        </div>

        <div className="grid gap-2 text-sm text-surface-foreground/80 sm:grid-cols-2">
          <p>
            <span className="font-semibold text-primary-foreground">Email:</span> {profile.email}
          </p>
          <p>
            <span className="font-semibold text-primary-foreground">Số điện thoại:</span> {profile.phone}
          </p>
          <p>
            <span className="font-semibold text-primary-foreground">Tham gia:</span> {formatDate(profile.created_at)}
          </p>
          <p>
            <span className="font-semibold text-primary-foreground">Cập nhật gần nhất:</span> {formatDate(profile.updated_at)}
          </p>
          {profile.banned_at && (
            <p className="sm:col-span-2">
              <span className="font-semibold text-primary-foreground">Thời điểm khóa:</span> {formatDate(profile.banned_at)}
            </p>
          )}
        </div>

        <div className="flex flex-wrap gap-2 text-xs text-surface-foreground/70">
          {profile.roles.map((role) => (
            <span key={role} className="rounded-full border border-primary/40 bg-primary/10 px-3 py-1 font-semibold uppercase tracking-wide text-primary">
              Vai trò: {role}
            </span>
          ))}
          {profile.permissions.length > 0 && (
            <span className="rounded-full border border-surface-muted/60 bg-surface px-3 py-1 font-semibold uppercase tracking-wide text-surface-foreground/80">
              Quyền hạn: {profile.permissions.length}
            </span>
          )}
        </div>
      </div>
    </section>
  );
};

export default ProfileHeader;
