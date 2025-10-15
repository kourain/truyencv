# Const

## Enum

thuộc thư mục ./enum
Luôn tạo Label để lấy thông tin của enum, ví dụ:
```ts
export enum ComicStatus {
	Continuing = 1,
	Paused = 2,
	Stopped = 3,
	Completed = 4,
	Banned = 5,
}

export const ComicStatusLabel: Record<ComicStatus, string> = {
  [ComicStatus.Continuing]: "Đang cập nhật",
  [ComicStatus.Paused]: "Tạm dừng",
  [ComicStatus.Stopped]: "Đã dừng",
  [ComicStatus.Banned]: "Cấm phát hành",
  [ComicStatus.Completed]: "Đã hoàn thành"
};
```
