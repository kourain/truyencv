"use client";

import { FormEvent, useMemo, useState, type ComponentType } from "react";
import { useQuery } from "@tanstack/react-query";
import { AlertCircle, CreditCard, Loader2, RefreshCcw, Search, TrendingUp, Wallet } from "lucide-react";

import { fetchPaymentHistories, fetchPaymentRevenueSummary } from "@services/admin";
import RevenueChart from "./RevenueChart";
import CreatePaymentHistoryForm from "./CreatePaymentHistoryForm";

const DEFAULT_LIMIT = 20;
const REVENUE_DAYS = 60;

const currencyFormatter = new Intl.NumberFormat("vi-VN", {
	style: "currency",
	currency: "VND",
	minimumFractionDigits: 0,
});

const numberFormatter = new Intl.NumberFormat("vi-VN");

const AdminPaymentsPage = () => {
	const [offset, setOffset] = useState(0);
	const [keywordInput, setKeywordInput] = useState("");
	const [keyword, setKeyword] = useState("");

	const paymentQuery = useQuery({
		queryKey: ["admin-payment-histories", offset, keyword],
		queryFn: () =>
			fetchPaymentHistories({
				offset,
				limit: DEFAULT_LIMIT,
				keyword: keyword.trim() === "" ? undefined : keyword.trim(),
			}),
	});

	const revenueQuery = useQuery({
		queryKey: ["admin-payment-revenue", REVENUE_DAYS],
		queryFn: () => fetchPaymentRevenueSummary(REVENUE_DAYS),
	});

	const tableRows = useMemo<PaymentHistoryResponse[]>(() => paymentQuery.data ?? [], [paymentQuery.data]);
	const hasNextPage = tableRows.length === DEFAULT_LIMIT;

	const totalCoin = useMemo(() => tableRows.reduce((sum, item) => sum + item.amount_coin, 0), [tableRows]);
	const totalMoney = useMemo(() => tableRows.reduce((sum, item) => sum + item.amount_money, 0), [tableRows]);

	const revenuePoints = useMemo<PaymentRevenuePointResponse[]>(() => revenueQuery.data ?? [], [revenueQuery.data]);
	const revenueTotalMoney = useMemo(
		() => revenuePoints.reduce((sum, point) => sum + point.total_amount_money, 0),
		[revenuePoints],
	);

	const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		setOffset(0);
		setKeyword(keywordInput.trim());
	};

	const handleResetFilters = () => {
		setKeyword("");
		setKeywordInput("");
		setOffset(0);
	};

	const handleCreatedHistory = () => {
		setOffset(0);
		paymentQuery.refetch();
		revenueQuery.refetch();
	};

	const formatDateTime = (date: string) => new Date(date).toLocaleString();

	return (
		<div className="space-y-10">
			<CreatePaymentHistoryForm onCreated={handleCreatedHistory} />
			<section className="space-y-4">
				<header className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
					<div>
						<p className="text-xs uppercase tracking-[0.4em] text-primary/70">Quản lý nạp tiền</p>
						<h2 className="text-2xl font-semibold text-primary-foreground">Lịch sử giao dịch</h2>
					</div>
					<div className="flex gap-2">
						<button
							type="button"
							onClick={() => paymentQuery.refetch()}
							className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
						>
							<RefreshCcw className="h-4 w-4" />
							Làm mới
						</button>
						<button
							type="button"
							onClick={handleResetFilters}
							className="inline-flex items-center gap-2 rounded-full border border-surface-muted px-4 py-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70 transition hover:border-primary/60 hover:text-primary-foreground"
						>
							<ResetIcon />
							Đặt lại
						</button>
					</div>
				</header>

				<form onSubmit={handleSubmit} className="flex flex-wrap items-center gap-3 rounded-2xl border border-surface-muted/70 bg-surface/60 p-4">
					<label className="flex flex-1 items-center gap-3 text-xs font-semibold uppercase tracking-wide text-surface-foreground/70">
						<span>Từ khóa</span>
						<span className="flex flex-1 items-center gap-2 rounded-xl border border-surface-muted bg-surface px-3 py-2 text-sm focus-within:ring-2 focus-within:ring-primary/50">
							<Search className="h-4 w-4 text-surface-foreground/50" />
							<input
								type="text"
								value={keywordInput}
								onChange={(event) => setKeywordInput(event.target.value)}
								placeholder="Nhập user id hoặc email"
								className="flex-1 bg-transparent text-sm text-surface-foreground outline-none"
							/>
						</span>
					</label>
					<button
						type="submit"
						className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold uppercase tracking-wide text-primary-foreground transition hover:bg-primary/90"
					>
						<Search className="h-4 w-4" />
						Tìm kiếm
					</button>
				</form>

				<section className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
					<SummaryCard
						icon={CreditCard}
						title="Giao dịch trong trang"
						value={`${tableRows.length}`}
						description="Dựa trên phân trang hiện tại"
					/>
					<SummaryCard
						icon={Wallet}
						title="Tổng xu"
						value={numberFormatter.format(totalCoin)}
						description="Tổng xu đã nạp"
					/>
					<SummaryCard
						icon={TrendingUp}
						title="Tổng tiền"
						value={currencyFormatter.format(totalMoney)}
						description="Theo danh sách hiện tại"
					/>
					<SummaryCard
						icon={CreditCard}
						title="Doanh thu 60 ngày"
						value={currencyFormatter.format(revenueTotalMoney)}
						description="Cộng dồn toàn hệ thống"
					/>
				</section>

				<div className="overflow-hidden rounded-lg border border-surface-muted/60 bg-surface/80 shadow">
					<table className="min-w-full text-sm">
						<thead className="bg-surface-muted/50 text-xs uppercase tracking-wide text-surface-foreground/60">
							<tr>
								<th scope="col" className="px-4 py-3 text-left font-semibold">#</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Người dùng</th>
								<th scope="col" className="px-4 py-3 text-right font-semibold">Số xu</th>
								<th scope="col" className="px-4 py-3 text-right font-semibold">Số tiền</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Phương thức</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Mã tham chiếu</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Ghi chú</th>
								<th scope="col" className="px-4 py-3 text-left font-semibold">Thời gian</th>
							</tr>
						</thead>
						<tbody className="divide-y divide-surface-muted/40">
							{paymentQuery.isLoading && (
								<tr>
									<td colSpan={8} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
										Đang tải lịch sử nạp tiền...
									</td>
								</tr>
							)}
							{paymentQuery.isError && (
								<tr>
									<td colSpan={8} className="px-4 py-6 text-center text-xs text-red-300">
										Không thể tải dữ liệu. Vui lòng thử lại.
									</td>
								</tr>
							)}
							{!paymentQuery.isLoading && !paymentQuery.isError && tableRows.length === 0 && (
								<tr>
									<td colSpan={8} className="px-4 py-6 text-center text-xs text-surface-foreground/60">
										Không có giao dịch nào.
									</td>
								</tr>
							)}
							{tableRows.map((item, index) => (
								<tr key={item.id} className="transition hover:bg-surface-muted/40">
									<td className="px-4 py-3 text-xs text-surface-foreground/60">{offset + index + 1}</td>
									<td className="px-4 py-3 text-xs text-surface-foreground/80">
										<div className="flex flex-col gap-1">
											<span className="font-semibold text-primary-foreground">{item.user_name ?? item.user_id}</span>
											<span className="text-[11px] text-surface-foreground/60">{item.user_email ?? "Không rõ email"}</span>
										</div>
									</td>
									<td className="px-4 py-3 text-right text-xs text-surface-foreground/80">{numberFormatter.format(item.amount_coin)}</td>
									<td className="px-4 py-3 text-right text-xs text-surface-foreground/80">{currencyFormatter.format(item.amount_money)}</td>
									<td className="px-4 py-3 text-xs text-surface-foreground/80">{item.payment_method ?? "Không rõ"}</td>
									<td className="px-4 py-3 text-xs text-surface-foreground/60">{item.reference_id ?? "-"}</td>
									<td className="px-4 py-3 text-xs text-surface-foreground/60 line-clamp-2" title={item.note ?? "Không có ghi chú"}>
										{item.note ?? "Không có ghi chú"}
									</td>
									<td className="px-4 py-3 text-xs text-surface-foreground/80">{formatDateTime(item.created_at)}</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>

				<div className="flex items-center justify-between text-xs text-surface-foreground/60">
					<button
						type="button"
						onClick={() => setOffset((prev) => Math.max(prev - DEFAULT_LIMIT, 0))}
						disabled={offset === 0 || paymentQuery.isLoading}
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
						disabled={!hasNextPage || paymentQuery.isLoading}
						className="rounded-md border border-surface-muted px-3 py-1 transition hover:border-primary/60 hover:text-primary-foreground disabled:opacity-60"
					>
						Trang tiếp
					</button>
				</div>
			</section>

			<section className="space-y-4 rounded-2xl border border-surface-muted bg-surface/70 p-6">
				<header className="flex items-center gap-2 text-primary-foreground">
					<TrendingUp className="h-5 w-5" />
					<h3 className="text-sm font-semibold uppercase tracking-[0.3em]">Doanh thu {REVENUE_DAYS} ngày</h3>
				</header>
				{revenueQuery.isLoading ? (
					<p className="flex items-center gap-2 text-sm text-surface-foreground/60">
						<Loader2 className="h-4 w-4 animate-spin" /> Đang tải dữ liệu doanh thu...
					</p>
				) : revenueQuery.isError ? (
					<p className="flex items-center gap-2 text-sm text-red-300">
						<AlertCircle className="h-4 w-4" /> Không thể tải dữ liệu doanh thu.
					</p>
				) : revenuePoints.length === 0 ? (
					<p className="text-sm text-surface-foreground/60">Chưa có dữ liệu doanh thu.</p>
				) : (
					<RevenueChart data={revenuePoints} />
				)}
			</section>
		</div>
	);
};

export default AdminPaymentsPage;

interface SummaryCardProps {
	title: string;
	description: string;
	value: string;
	icon: ComponentType<{ className?: string }>;
}

const SummaryCard = ({ title, description, value, icon: Icon }: SummaryCardProps) => (
	<div className="rounded-2xl border border-surface-muted/70 bg-surface/80 p-4">
		<div className="flex items-center justify-between">
			<div className="space-y-1">
				<p className="text-xs uppercase tracking-wide text-surface-foreground/60">{title}</p>
				<p className="text-lg font-semibold text-primary-foreground">{value}</p>
			</div>
			<span className="inline-flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
				<Icon className="h-5 w-5" />
			</span>
		</div>
		<p className="mt-2 text-xs text-surface-foreground/60">{description}</p>
	</div>
);

const ResetIcon = () => (
	<span className="relative inline-flex h-4 w-4 items-center justify-center">
		<span className="absolute inline-flex h-4 w-4 animate-spin rounded-full border border-surface-muted/70 border-t-transparent" />
		<RefreshCcw className="h-3 w-3" />
	</span>
);
