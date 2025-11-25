export const formatRelativeTime = (isoString: string) => {
  const date = new Date(isoString);
  const now = Date.now();
  let diff = now - date.getTime();

  const minute = 60 * 1000;
  const hour = 60 * minute;
  const day = 24 * hour;
  const week = 7 * day;
  const month = 30 * day;
  const MAX_MONTH = 3 * month;
  if (diff < 0) {
    diff = -diff;
    if (diff < minute) {
      return "trong 1 phút";
    }

    if (diff < hour) {
      const count = Math.floor(diff / minute);
      return `sau ${count} phút`;
    }

    if (diff < day) {
      const count = Math.floor(diff / hour);
      return `sau ${count} giờ`;
    }

    if (diff < week) {
      const count = Math.floor(diff / day);
      return `sau ${count} ngày`;
    }
    if (diff < month) {
      const count = Math.floor(diff / week);
      return `sau ${count} tuần`;
    }
    if (diff < MAX_MONTH) {
      const count = Math.floor(diff / month);
      return `sau ${count} tháng`;
    }

    return date.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  }
  else {
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
    if (diff < month) {
      const count = Math.floor(diff / week);
      return `${count} tuần trước`;
    }
    if (diff < MAX_MONTH) {
      const count = Math.floor(diff / month);
      return `${count} tháng trước`;
    }

    return date.toLocaleDateString("vi-VN", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    });
  }
};

export const formatNumber = (value: number, options?: Intl.NumberFormatOptions) => {
  return new Intl.NumberFormat("vi-VN", options).format(value);
};
