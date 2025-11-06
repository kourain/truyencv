"use client";

import { useEffect, useMemo, useState } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { AlertCircle, ClipboardList, EyeOff, RefreshCcw, ShieldQuestion, UserCircle2 } from "lucide-react";

import { fetchComicReportById, fetchComicReports } from "@services/admin";
import { ReportStatus, ReportStatusLabel } from "@const/enum/report-status";

import ReportStatusBadge from "./ReportStatusBadge";
import ReportActions from "./ReportActions";
import { ComicReportResponse } from "../../../types/comic-report";

const DEFAULT_LIMIT = 20;

type StatusFilterValue = "" | ReportStatus;

const statusFilterOptions: { label: string; value: StatusFilterValue }[] = [
	{ label: "Tất cả", value: "" },
	{ label: ReportStatusLabel[ReportStatus.Pending], value: ReportStatus.Pending },
	{ label: ReportStatusLabel[ReportStatus.InProgress], value: ReportStatus.InProgress },
	{ label: ReportStatusLabel[ReportStatus.Resolved], value: ReportStatus.Resolved },
	{ label: ReportStatusLabel[ReportStatus.Rejected], value: ReportStatus.Rejected },
];

const AdminReportsPage = () => {
	const queryClient = useQueryClient();
	const [offset, setOffset] = useState(0);
	const [statusFilter, setStatusFilter] = useState<StatusFilterValue>("");
	const [selectedReportId, setSelectedReportId] = useState<string | null>(null);

	useEffect(() => {
		setOffset(0);
	}, [statusFilter]);

	const reportsQuery = useQuery({
		queryKey: ["admin-comic-reports", offset, statusFilter],
		queryFn: () =>
			fetchComicReports({
				offset,
				limit: DEFAULT_LIMIT,
				status: statusFilter,
			}),
	});

	const selectedReportQuery = useQuery({
		queryKey: ["admin-comic-report", selectedReportId],
		queryFn: () => fetchComicReportById(selectedReportId!),
		enabled: selectedReportId !== null,
	});

	const handleRefresh = () => {
		queryClient.invalidateQueries({ queryKey: ["admin-comic-reports"] });
		if (selectedReportId) {
			queryClient.invalidateQueries({ queryKey: ["admin-comic-report", selectedReportId] });
		}
	};

	const renderStatus = (status: ReportStatus) => <ReportStatusBadge status={status} />;

	const formatDateTime = (date: string) => new Date(date).toLocaleString();

	const tableRows = useMemo<ComicReportResponse[]>(() => reportsQuery.data ?? [], [reportsQuery.data]);

	useEffect(() => {
		if (tableRows.length === 0) {
			setSelectedReportId(null);
			return;
		}

		if (!selectedReportId || !tableRows.some((report) => report.id === selectedReportId)) {
			setSelectedReportId(tableRows[0].id);
		}
	}, [tableRows, selectedReportId]);

	const hasNextPage = tableRows.length === DEFAULT_LIMIT;

	return (
		<div className="space-y-10">
			<section className="space-y-4">
				<header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
					<div>
						<p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý báo cáo</p>
						<h2 className="text-2xl font-semibold text-primary-foreground">Báo cáo cộng đồng</h2>
					</div>
					<div className="flex gap-2">
						<button
							type="button"
							onClick={handleRefresh}
							className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
						>
							<RefreshCcw className="h-4 w-4" />
							Làm mới
						</button>
					</div>
				</header>
				<div className="flex flex-wrap items-center gap-3 rounded-2xl border border-surface-muted/70 bg-surface/60 p-4">
					<label className="flex items-center gap-3 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70">
						<span>Trạng thái</span>
						<select
							value={statusFilter}
							onChange={(event) => {
								const nextValue = event.target.value === "" ? "" : (Number(event.target.value) as ReportStatus);
								setStatusFilter(nextValue);
							}}
							className="rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary/50"
						>
							{statusFilterOptions.map((option) => (
								<option key={option.value === "" ? "all" : option.value} value={option.value}>
									{option.label}
								</option>
							))}
						</select>
					</label>
				</div>
				<div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
					<table className="min-w-full text-sm">
						<thead className="bg-surface-muted/50 text-xs uppercase tracking-wide text-surface-foreground/60">
							<tr>
								<th scope="col" className="px-4 py-3 text-left font-semibold">#</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Truyện / Bình luận</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Người báo cáo</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Lý do</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Trạng thái</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Thời gian</th>
							</tr>
						</thead>
						<tbody className="divide-y divide-surface-muted/40">
							{reportsQuery.isLoading && (
								<tr>
									<td colSpan={6} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
										Đang tải danh sách báo cáo...
									</td>
								</tr>
							)}
							{reportsQuery.isError && (
								<tr>
									<td colSpan={6} className="px-4 py-6 text-center text-xs text-red-300">
										Không thể tải báo cáo. Vui lòng thử lại.
									</td>
								</tr>
							)}
							{!reportsQuery.isLoading && !reportsQuery.isError && tableRows.length === 0 && (
								<tr>
									<td colSpan={6} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
										Chưa có báo cáo nào.
									</td>
								</tr>
							)}
							{tableRows.map((report, index) => {
								const isActive = selectedReportId === report.id;
								return (
									<tr
										key={report.id}
										className={`cursor-pointer transition ${
											isActive ? "bg-primary/15 text-primary-foreground" : "hover:bg-surface-muted/40 text-surface-foreground/80"
										}`}
										onClick={() => setSelectedReportId(report.id)}
									>
										<td className="px-4 py-3 align-middle text-xs text-surface-foreground/60">{offset + index + 1}</td>
										<td className="px-4 py-3 align-middle">
											<div className="flex flex-col gap-1">
												<span className="font-semibold">{report.comic_name ?? `Truyện #${report.comic_id}`}</span>
												{report.comment_content ? (
													<p className="text-xs italic text-surface-foreground/60 line-clamp-2" title={report.comment_content}>
														"{report.comment_content}"
													</p>
												) : (
													<span className="text-xs text-surface-foreground/60">Báo cáo truyện</span>
												)}
											</div>
										</td>
										<td className="px-4 py-3 align-middle">
											<div className="flex items-center gap-2 text-xs text-surface-foreground/70">
												<UserCircle2 className="h-4 w-4" />
												<div className="flex flex-col">
													<span>{report.reporter_name ?? report.reporter_id}</span>
													<span className="text-[10px] text-surface-foreground/50">{report.reporter_email ?? "Không rõ email"}</span>
												</div>
											</div>
										</td>
										<td className="px-4 py-3 align-middle text-xs text-surface-foreground/70 line-clamp-2" title={report.reason}>
											{report.reason}
										</td>
										<td className="px-4 py-3 align-middle">{renderStatus(report.status)}</td>
										<td className="px-4 py-3 align-middle text-xs text-surface-foreground/70">
											{formatDateTime(report.created_at)}
										</td>
									</tr>
								);
							})}
						</tbody>
					</table>
				</div>
				<div className="flex items-center justify-between text-xs text-surface-foreground/60">
					<button
						type="button"
						onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
						disabled={offset === 0}
						className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
					>
						Trang trước
					</button>
					<span>
						{offset + 1} - {offset + DEFAULT_LIMIT}
					</span>
					<button
						type="button"
						onClick={() => setOffset((prev) => prev + DEFAULT_LIMIT)}
						disabled={!hasNextPage}
						className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
					>
						Trang tiếp
					</button>
				</div>
			</section>

			<section className="grid gap-6 rounded-2xl border border-surface-muted bg-surface/70 p-6 md:grid-cols-[0.6fr_1.4fr]">
				<div className="space-y-3">
					<header className="flex items-center gap-2 text-primary-foreground">
						<ShieldQuestion className="h-5 w-5" />
						<h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Chi tiết báo cáo</h3>
					</header>
					{selectedReportId === null ? (
						<p className="text-sm text-surface-foreground/60">Chọn một báo cáo để xem chi tiết.</p>
					) : selectedReportQuery.isLoading ? (
						<p className="text-sm text-surface-foreground/60">Đang tải thông tin báo cáo...</p>
					) : selectedReportQuery.isError ? (
						<p className="flex items-center gap-2 text-sm text-red-300">
							<AlertCircle className="h-4 w-4" /> Không thể tải báo cáo.
						</p>
					) : selectedReportQuery.data ? (
						<article className="space-y-4 rounded-2xl border border-surface-muted/70 bg-surface px-4 py-4 text-sm text-surface-foreground/80">
							<div className="flex items-center justify-between">
								<div className="space-y-1">
									<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Trạng thái</p>
									{renderStatus(selectedReportQuery.data.status)}
								</div>
								<p className="text-xs text-surface-foreground/60">
									Cập nhật: {formatDateTime(selectedReportQuery.data.updated_at)}
								</p>
							</div>
							<div className="grid gap-2">
								<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Truyện</p>
								<div className="rounded-xl border border-surface-muted/60 bg-surface px-3 py-2 text-sm">
									<p className="font-semibold text-primary-foreground">
										{selectedReportQuery.data.comic_name ?? `Truyện #${selectedReportQuery.data.comic_id}`}
									</p>
									<p className="text-xs text-surface-foreground/60">Mã truyện: {selectedReportQuery.data.comic_id}</p>
									{selectedReportQuery.data.comic_status !== null && (
										<p className="mt-1 text-xs text-surface-foreground/60">
											Trạng thái truyện: {selectedReportQuery.data.comic_status}
										</p>
									)}
								</div>
							</div>
							<div className="grid gap-2">
								<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Lý do báo cáo</p>
								<div className="rounded-xl border border-surface-muted/60 bg-surface px-3 py-2 text-sm leading-relaxed">
									{selectedReportQuery.data.reason}
								</div>
							</div>
							{selectedReportQuery.data.comment_content && (
								<div className="grid gap-2">
									<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Bình luận liên quan</p>
									<div className="space-y-2 rounded-xl border border-surface-muted/60 bg-surface px-3 py-3 text-sm">
										<p className="leading-relaxed">{selectedReportQuery.data.comment_content}</p>
										{selectedReportQuery.data.comment_is_hidden ? (
											<span className="inline-flex items-center gap-1 rounded-full bg-amber-100 px-3 py-1 text-[11px] font-semibold uppercase tracking-wide text-amber-700">
												<EyeOff className="h-3 w-3" /> Đã được ẩn
											</span>
										) : null}
									</div>
								</div>
							)}
							<div className="grid gap-2">
								<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Người báo cáo</p>
								<div className="rounded-xl border border-surface-muted/60 bg-surface px-3 py-2 text-sm">
									<p className="font-semibold text-primary-foreground">
										{selectedReportQuery.data.reporter_name ?? selectedReportQuery.data.reporter_id}
									</p>
									<p className="text-xs text-surface-foreground/60">
										{selectedReportQuery.data.reporter_email ?? "Không rõ email"}
									</p>
								</div>
							</div>
							<ReportActions
								reportId={selectedReportQuery.data.id}
								currentStatus={selectedReportQuery.data.status}
								disableCommentActions={!selectedReportQuery.data.comment_id}
							/>
						</article>
					) : null}
				</div>
				<div className="rounded-2xl border border-surface-muted bg-surface/60 p-6 text-sm text-surface-foreground/70">
					<div className="flex items-center gap-2 text-primary-foreground">
						<ClipboardList className="h-5 w-5" />
						<h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Ghi chú nhanh</h3>
					</div>
					<ul className="mt-4 space-y-2 text-xs leading-relaxed text-surface-foreground/60">
						<li>- Sử dụng hành động "Cấm truyện" để chuyển trạng thái truyện sang bị cấm.</li>
						<li>- "Ẩn bình luận" chỉ áp dụng với báo cáo bình luận, dữ liệu vẫn được giữ lại.</li>
						<li>- Sau khi xử lý, đánh dấu "Đã xử lý" để thông báo cho hệ thống.</li>
					</ul>
				</div>
			</section>
		</div>
	);
};

export default AdminReportsPage;
