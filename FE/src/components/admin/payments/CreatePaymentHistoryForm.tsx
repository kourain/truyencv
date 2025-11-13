"use client";

import { FormEvent, useMemo, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { Loader2, PlusCircle } from "lucide-react";
import type { AxiosError } from "axios";

import { useToast } from "@components/providers/ToastProvider";
import { createPaymentHistory } from "@services/admin";
import { formatNumber } from "@helpers/format";

interface CreatePaymentHistoryFormProps {
	onCreated?: () => void;
}

const initialFormState = {
	user_id: "",
	amount_coin: "",
	amount_money: "",
	payment_method: "",
	reference_id: "",
	note: "",
};

type FormState = typeof initialFormState;

const CreatePaymentHistoryForm = ({ onCreated }: CreatePaymentHistoryFormProps) => {
	const { pushToast } = useToast();
	const [form, setForm] = useState<FormState>(initialFormState);
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	const mutation = useMutation({
		mutationFn: (payload: CreatePaymentHistoryRequest) => createPaymentHistory(payload),
		onSuccess: () => {
			pushToast({
				title: "Thành công",
				description: "Đã tạo giao dịch nạp tiền mới.",
				variant: "success",
			});
			setErrorMessage(null);
			setForm(initialFormState);
			onCreated?.();
		},
		onError: (error: unknown) => {
			const message = resolveErrorMessage(error);
			setErrorMessage(message);
			pushToast({
				title: "Không thể tạo giao dịch",
				description: message,
				variant: "error",
			});
		},
	});

	const isSubmitting = mutation.isPending;

	const formattedPreview = useMemo(() => {
		const coin = form.amount_coin ? Number(form.amount_coin) : 0;
		const money = form.amount_money ? Number(form.amount_money) : 0;

		return {
			coin: coin > 0 ? formatNumber(coin) : "0",
			money: money > 0 ? formatCurrency(money) : formatCurrency(0),
		};
	}, [form.amount_coin, form.amount_money]);

	const handleChange = (field: keyof FormState) => (value: string) => {
		setForm((prev) => ({ ...prev, [field]: value }));
	};

	const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
		event.preventDefault();

		const parsedCoin = Number(form.amount_coin);
		const parsedMoney = Number(form.amount_money);

		if (!form.user_id.trim()) {
			setErrorMessage("Vui lòng nhập ID người dùng.");
			return;
		}

		if (!Number.isFinite(parsedCoin) || parsedCoin <= 0) {
			setErrorMessage("Số xu phải lớn hơn 0.");
			return;
		}

		if (!Number.isFinite(parsedMoney) || parsedMoney <= 0) {
			setErrorMessage("Số tiền phải lớn hơn 0.");
			return;
		}

		setErrorMessage(null);

		mutation.mutate({
			user_id: form.user_id.trim(),
			amount_coin: parsedCoin,
			amount_money: parsedMoney,
			payment_method: form.payment_method.trim() || undefined,
			reference_id: form.reference_id.trim() || null,
			note: form.note.trim() || null,
		});
	};

	return (
		<section className="space-y-4 rounded-2xl border border-surface-muted bg-surface/70 p-6">
			<header className="flex items-center justify-between">
				<div>
					<p className="text-xs uppercase tracking-[0.4em] text-primary/70">Nạp thủ công</p>
					<h3 className="text-lg font-semibold text-primary-foreground">Tạo giao dịch nạp mới</h3>
				</div>
			</header>
			<form onSubmit={handleSubmit} className="grid gap-4 md:grid-cols-2">
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60 md:col-span-2">
					<span>ID người dùng</span>
					<input
						type="text"
						value={form.user_id}
						onChange={(event) => handleChange("user_id")(event.target.value)}
						required
						disabled={isSubmitting}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="Ví dụ: 92384756231"
					/>
				</label>
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60">
					<span>Số xu</span>
					<input
						type="number"
						min="1"
						step="1"
						value={form.amount_coin}
						onChange={(event) => handleChange("amount_coin")(event.target.value)}
						required
						disabled={isSubmitting}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="1000"
					/>
				</label>
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60">
					<span>Số tiền (VNĐ)</span>
					<input
						type="number"
						min="1000"
						step="1000"
						value={form.amount_money}
						onChange={(event) => handleChange("amount_money")(event.target.value)}
						required
						disabled={isSubmitting}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="50000"
					/>
				</label>
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60">
					<span>Phương thức</span>
					<input
						type="text"
						value={form.payment_method}
						onChange={(event) => handleChange("payment_method")(event.target.value)}
						disabled={isSubmitting}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="Ví dụ: Momo"
					/>
				</label>
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60">
					<span>Mã tham chiếu</span>
					<input
						type="text"
						value={form.reference_id}
						onChange={(event) => handleChange("reference_id")(event.target.value)}
						disabled={isSubmitting}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="Ref#12345"
					/>
				</label>
				<label className="flex flex-col gap-2 text-xs font-semibold uppercase tracking-wide text-surface-foreground/60 md:col-span-2">
					<span>Ghi chú</span>
					<textarea
						value={form.note}
						onChange={(event) => handleChange("note")(event.target.value)}
						disabled={isSubmitting}
						rows={3}
						className="rounded-xl border border-surface-muted bg-surface px-4 py-2.5 text-sm text-primary-foreground outline-none transition focus:border-primary focus:ring-2 focus:ring-primary/30 disabled:opacity-60"
						placeholder="Ghi chú nội bộ..."
					/>
				</label>
				<div className="md:col-span-2">
					<p className="text-[11px] uppercase tracking-wide text-surface-foreground/50">Xem trước</p>
					<div className="mt-2 flex flex-wrap items-center gap-4 text-xs text-surface-foreground/70">
						<span>Xu: <strong className="text-primary-foreground">{formattedPreview.coin}</strong></span>
						<span>Số tiền: <strong className="text-primary-foreground">{formattedPreview.money}</strong></span>
					</div>
				</div>
				<div className="md:col-span-2">
					<button
						type="submit"
						disabled={isSubmitting}
						className="inline-flex items-center gap-2 rounded-full bg-primary px-5 py-2 text-xs font-semibold uppercase tracking-wide text-primary-foreground transition hover:bg-primary/90 disabled:opacity-70"
					>
						{isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : <PlusCircle className="h-4 w-4" />}
						Tạo giao dịch
					</button>
				</div>
			</form>
			{errorMessage && <p className="text-xs text-rose-300">{errorMessage}</p>}
		</section>
	);
};

const resolveErrorMessage = (error: unknown) => {
	if (typeof error === "object" && error !== null && "response" in error) {
		const apiError = error as AxiosError<{ message?: string }>;
		return apiError.response?.data?.message?.trim() || "Không thể tạo giao dịch. Vui lòng thử lại.";
	}

	if (error instanceof Error && error.message) {
		return error.message;
	}

	return "Không thể tạo giao dịch. Vui lòng thử lại.";
};

const formatCurrency = (value: number) =>
	new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND", minimumFractionDigits: 0 }).format(value);

export default CreatePaymentHistoryForm;
