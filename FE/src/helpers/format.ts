export const formatRelativeTime = (isoString: string) => {
  const date = new Date(isoString);
  const now = Date.now();
  const diff = now - date.getTime();

  const minute = 60 * 1000;
  const hour = 60 * minute;
  const day = 24 * hour;
  const week = 7 * day;

  if (diff < minute) {
    return "vừa xong";
  }

  if (diff < hour) {
    const count = Math.floor(diff / minute);
    return `${count} phút trước`;
  }

  if (diff < day) {
    const count = Math.floor(diff / hour);
    return `${count} giờ trước`;
  }

  if (diff < week) {
    const count = Math.floor(diff / day);
    return `${count} ngày trước`;
  }

  return date.toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
};

export const formatNumber = (value: number, options?: Intl.NumberFormatOptions) => {
  return new Intl.NumberFormat("vi-VN", options).format(value);
};
