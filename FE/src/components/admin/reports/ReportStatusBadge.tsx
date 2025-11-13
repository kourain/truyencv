import type { ReportStatus } from "@const/enum/report-status";
import { ReportStatusLabel } from "@const/enum/report-status";

const statusStyles: Record<ReportStatus, string> = {
	1: "bg-yellow-100 text-yellow-800 border-yellow-200",
	2: "bg-amber-100 text-amber-800 border-amber-200",
	3: "bg-emerald-100 text-emerald-800 border-emerald-200",
	4: "bg-rose-100 text-rose-800 border-rose-200",
};

interface ReportStatusBadgeProps {
	status: ReportStatus;
}

const ReportStatusBadge = ({ status }: ReportStatusBadgeProps) => {
	const label = ReportStatusLabel[status];
	const style = statusStyles[status];

	return (
		<span className={`inline-flex items-center rounded-full border px-3 py-1 text-xs font-semibold ${style}`}>
			{label}
		</span>
	);
};

export default ReportStatusBadge;
