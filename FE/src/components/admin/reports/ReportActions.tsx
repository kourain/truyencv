"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, BookX, EyeOff, Loader2, ShieldCheck } from "lucide-react";
import type { AxiosError } from "axios";

import {
	banComicFromReport,
	hideCommentFromReport,
	updateComicReportStatus,
} from "@services/admin";
import { ReportStatus } from "@const/enum/report-status";
import { useToast } from "@components/providers/ToastProvider";

interface ReportActionsProps {
	reportId: string;
	currentStatus: ReportStatus;
	disableCommentActions?: boolean;
}

const ReportActions = ({ reportId, currentStatus, disableCommentActions }: ReportActionsProps) => {
	const queryClient = useQueryClient();
	const { pushToast } = useToast();

	const invalidateQueries = () => {
		queryClient.invalidateQueries({ queryKey: ["admin-comic-reports"] });
		queryClient.invalidateQueries({ queryKey: ["admin-comic-report", reportId] });
	};

	const resolveErrorMessage = (error: unknown) => {
		if (typeof error === "object" && error !== null && "response" in error) {
			const apiError = error as AxiosError<{ message?: string }>;
			return apiError.response?.data?.message?.trim() || "Không thể hoàn tất thao tác. Vui lòng thử lại.";
		}

		if (error instanceof Error && error.message) {
			return error.message;
		}

		return "Không thể hoàn tất thao tác. Vui lòng thử lại.";
	};

	const showSuccessToast = (description: string) => {
		pushToast({
			title: "Thành công",
			description,
			variant: "success",
		});
	};

	const showErrorToast = (error: unknown) => {
		pushToast({
			title: "Không thể hoàn tất thao tác",
			description: resolveErrorMessage(error),
			variant: "error",
		});
	};

	const statusMutation = useMutation({
		mutationFn: (status: ReportStatus) => updateComicReportStatus({ id: reportId, status }),
		onSuccess: (_, status) => {
			invalidateQueries();
			switch (status) {
				case ReportStatus.Resolved:
					showSuccessToast("Đã đánh dấu báo cáo ở trạng thái đã xử lý.");
					break;
				case ReportStatus.Rejected:
					showSuccessToast("Đã từ chối báo cáo này.");
					break;
				default:
					showSuccessToast("Đã cập nhật trạng thái báo cáo.");
			}
		},
		onError: (error) => {
			invalidateQueries();
			showErrorToast(error);
		},
	});

	const banMutation = useMutation({
		mutationFn: () => banComicFromReport(reportId),
		onSuccess: () => {
			invalidateQueries();
			showSuccessToast("Đã cấm truyện theo báo cáo.");
		},
		onError: (error) => {
			invalidateQueries();
			showErrorToast(error);
		},
	});

	const hideMutation = useMutation({
		mutationFn: () => hideCommentFromReport(reportId),
		onSuccess: () => {
			invalidateQueries();
			showSuccessToast("Đã ẩn bình luận liên quan đến báo cáo.");
		},
		onError: (error) => {
			invalidateQueries();
			showErrorToast(error);
		},
	});

	const isLoading = statusMutation.isPending || banMutation.isPending || hideMutation.isPending;

	return (
		<div className="flex flex-wrap gap-2">
			<button
				type="button"
				onClick={() => statusMutation.mutate(ReportStatus.Resolved)}
				disabled={isLoading}
				className="inline-flex items-center gap-2 rounded-full border border-primary/40 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-primary transition hover:bg-primary/10 disabled:cursor-not-allowed"
			>
				{statusMutation.isPending ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <ShieldCheck className="h-3.5 w-3.5" />}
				Đánh dấu đã xử lý
			</button>
			<button
				type="button"
				onClick={() => banMutation.mutate()}
				disabled={isLoading}
				className="inline-flex items-center gap-2 rounded-full border border-red-500/50 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-red-200 transition hover:bg-red-500/10 disabled:cursor-not-allowed"
			>
				{banMutation.isPending ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <BookX className="h-3.5 w-3.5" />}
				Cấm truyện
			</button>
			<button
				type="button"
				onClick={() => {
					if (!disableCommentActions) {
						hideMutation.mutate();
					}
				}}
				disabled={isLoading || disableCommentActions}
				className="inline-flex items-center gap-2 rounded-full border border-amber-500/50 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-amber-200 transition hover:bg-amber-500/10 disabled:cursor-not-allowed"
			>
				{hideMutation.isPending ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <EyeOff className="h-3.5 w-3.5" />}
				Ẩn bình luận
			</button>
			<button
				type="button"
				onClick={() => statusMutation.mutate(ReportStatus.Rejected)}
				disabled={isLoading}
				className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-3 py-1 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground disabled:cursor-not-allowed"
			>
				{isLoading ? <Loader2 className="h-3.5 w-3.5 animate-spin" /> : <AlertTriangle className="h-3.5 w-3.5" />}
				Từ chối
			</button>
		</div>
	);
};

export default ReportActions;
