export enum ReportStatus {
	Pending = 1,
	InProgress = 2,
	Resolved = 3,
	Rejected = 4,
}

export const ReportStatusLabel: Record<ReportStatus, string> = {
	[ReportStatus.Pending]: "Chờ xử lý",
	[ReportStatus.InProgress]: "Đang xử lý",
	[ReportStatus.Resolved]: "Đã giải quyết",
	[ReportStatus.Rejected]: "Từ chối",
};
