
const currencyFormatter = new Intl.NumberFormat("vi-VN", {
	style: "currency",
	currency: "VND",
	minimumFractionDigits: 0,
});

const numberFormatter = new Intl.NumberFormat("vi-VN");

interface RevenueChartProps {
	data: PaymentRevenuePointResponse[];
}

const RevenueChart = ({ data }: RevenueChartProps) => {
	if (data.length === 0) {
		return null;
	}

	const maxValue = Math.max(...data.map((point) => point.total_amount_money), 1);
	const pointStep = data.length > 1 ? 100 / (data.length - 1) : 0;

	const chartPoints = data.map((point, index) => {
		const x = Number((index * pointStep).toFixed(2));
		const valueRatio = point.total_amount_money / maxValue;
		const y = Number((100 - valueRatio * 90 - 5).toFixed(2));
		return { x, y, point };
	});

	const linePath = chartPoints
		.map((item, index) => `${index === 0 ? "M" : "L"} ${item.x} ${item.y}`)
		.join(" ");

	const areaPath = `${linePath} L ${chartPoints.at(-1)?.x ?? 100} 100 L ${chartPoints.at(0)?.x ?? 0} 100 Z`;

	const totalMoney = data.reduce((sum, point) => sum + point.total_amount_money, 0);
	const totalCoin = data.reduce((sum, point) => sum + point.total_amount_coin, 0);
	const peak = data.reduce((previous, current) => (current.total_amount_money > previous.total_amount_money ? current : previous));
	const latest = data[data.length - 1];

	return (
		<div className="space-y-6">
			<div className="grid gap-4 md:grid-cols-2">
				<div className="rounded-xl border border-surface-muted/60 bg-surface px-4 py-3">
					<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Tổng doanh thu</p>
					<p className="text-lg font-semibold text-primary-foreground">{currencyFormatter.format(totalMoney)}</p>
					<p className="text-xs text-surface-foreground/60">Tương ứng {numberFormatter.format(totalCoin)} xu</p>
				</div>
				<div className="rounded-xl border border-surface-muted/60 bg-surface px-4 py-3">
					<p className="text-xs uppercase tracking-wide text-surface-foreground/60">Ngày cao nhất</p>
					<p className="text-lg font-semibold text-primary-foreground">{currencyFormatter.format(peak.total_amount_money)}</p>
					<p className="text-xs text-surface-foreground/60">{formatDate(peak.date)} - {numberFormatter.format(peak.total_amount_coin)} xu</p>
				</div>
			</div>
			<div className="h-72 w-full rounded-2xl border border-surface-muted/70 bg-surface/80 p-4">
				<svg viewBox="0 0 100 100" preserveAspectRatio="none" className="h-full w-full">
					<defs>
						<linearGradient id="revenueChartFill" x1="0" x2="0" y1="0" y2="1">
							<stop offset="0%" stopColor="var(--color-primary)" stopOpacity="0.35" />
							<stop offset="100%" stopColor="var(--color-primary)" stopOpacity="0" />
						</linearGradient>
					</defs>
					<g>
						{[25, 50, 75].map((position) => (
							<line
								key={position}
								x1="0"
								y1={position}
								x2="100"
								y2={position}
								stroke="var(--color-surface-muted)"
								strokeOpacity="0.3"
								strokeWidth="0.4"
							/>
						))}
						<path d={areaPath} fill="url(#revenueChartFill)" stroke="none" />
						<path d={linePath} fill="none" stroke="var(--color-primary)" strokeWidth="1.2" strokeLinejoin="round" strokeLinecap="round" />
						{chartPoints.map(({ x, y, point }) => (
							<circle key={point.date} cx={x} cy={y} r="1.6" fill="var(--color-primary)" />
						))}
					</g>
				</svg>
			</div>
			<div className="grid gap-2 text-xs text-surface-foreground/60 md:grid-cols-3">
				<div>
					<p className="font-semibold text-primary-foreground">Ngày đầu tiên</p>
					<p>{formatDate(data[0].date)}</p>
					<p>{currencyFormatter.format(data[0].total_amount_money)}</p>
				</div>
				<div>
					<p className="font-semibold text-primary-foreground">Ngày gần nhất</p>
					<p>{formatDate(latest.date)}</p>
					<p>{currencyFormatter.format(latest.total_amount_money)}</p>
				</div>
				<div>
					<p className="font-semibold text-primary-foreground">Số ngày có doanh thu</p>
					<p>{numberFormatter.format(data.filter((point) => point.total_amount_money > 0).length)}</p>
				</div>
			</div>
		</div>
	);
};

const formatDate = (value: string) => {
	const parsed = new Date(value);
	return parsed.toLocaleDateString("vi-VN", {
		day: "2-digit",
		month: "2-digit",
		year: "numeric",
	});
};

export default RevenueChart;
