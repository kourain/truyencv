"use client";

import { useState, useMemo } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import Image from "next/image";
import { ChevronLeft, Search, Lock, CheckCircle2, BookOpen, ArrowUpDown } from "lucide-react";

import { useUserComicChaptersListQuery } from "@services/user/comic-chapters-list.service";
import { formatRelativeTime } from "@helpers/format";

type SortOrder = "desc" | "asc";

const ComicChaptersListPage = () => {
	const params = useParams();
	const router = useRouter();
	const slug = params?.slug as string;

	const [searchQuery, setSearchQuery] = useState("");
	const [currentPage, setCurrentPage] = useState(1);
	const [sortOrder, setSortOrder] = useState<SortOrder>("desc");
	const chaptersPerPage = 50;

	const { data, isLoading, error } = useUserComicChaptersListQuery(slug);

	const filteredChapters = useMemo(() => {
		if (!data?.chapters) return [];

		let chapters = [...data.chapters];

		if (searchQuery.trim()) {
			const query = searchQuery.toLowerCase();
			chapters = chapters.filter(
				(chapter) =>
					chapter.chapter.toString().includes(query) || chapter.title.toLowerCase().includes(query)
			);
		}

		chapters.sort((a, b) => {
			if (sortOrder === "desc") {
				return b.chapter - a.chapter;
			}
			return a.chapter - b.chapter;
		});

		return chapters;
	}, [data?.chapters, searchQuery, sortOrder]);

	const paginatedChapters = useMemo(() => {
		const startIndex = (currentPage - 1) * chaptersPerPage;
		const endIndex = startIndex + chaptersPerPage;
		return filteredChapters.slice(startIndex, endIndex);
	}, [filteredChapters, currentPage, chaptersPerPage]);

	const totalPages = Math.ceil(filteredChapters.length / chaptersPerPage);

	const handlePageChange = (page: number) => {
		setCurrentPage(page);
		window.scrollTo({ top: 0, behavior: "smooth" });
	};

	const toggleSortOrder = () => {
		setSortOrder((prev) => (prev === "desc" ? "asc" : "desc"));
		setCurrentPage(1);
	};

	if (error) {
		return (
			<div className="container mx-auto max-w-7xl px-4 py-12">
				<div className="rounded-3xl border border-destructive/50 bg-destructive/10 p-8 text-center">
					<p className="text-lg font-semibold text-destructive">Không thể tải danh sách chương</p>
					<p className="mt-2 text-sm text-destructive/80">Vui lòng thử lại sau</p>
				</div>
			</div>
		);
	}

	return (
		<div className="min-h-screen bg-gradient-to-b from-background to-surface/30">
			<div className="container mx-auto max-w-7xl px-4 py-8">
				<button
					onClick={() => router.back()}
					className="mb-6 inline-flex items-center gap-2 rounded-full border border-surface-muted/60 bg-surface/80 px-4 py-2 text-sm font-semibold text-primary-foreground transition hover:bg-surface"
				>
					<ChevronLeft className="h-4 w-4" />
					Quay lại
				</button>

				{isLoading ? (
					<LoadingSkeleton />
				) : (
					<>
						<div className="mb-8 flex flex-col gap-6 rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 shadow-lg lg:flex-row lg:items-center">
							<div className="flex items-center gap-4">
								<div className="relative h-32 w-24 flex-shrink-0 overflow-hidden rounded-2xl border border-surface-muted/60 shadow-md">
									<Image
										src={data?.comic.cover_url || "/placeholder-cover.jpg"}
										alt={data?.comic.title || "Bìa truyện"}
										fill
										className="object-cover"
										sizes="96px"
									/>
								</div>
								<div className="flex flex-col gap-1">
									<h1 className="text-2xl font-bold text-primary-foreground">{data?.comic.title}</h1>
									<p className="text-sm text-surface-foreground/70">Tác giả: {data?.comic.author_name}</p>
									<p className="text-sm font-medium text-primary">
										Tổng số chương: {data?.total_chapters || 0}
									</p>
									{data?.user_last_read_chapter && (
										<div className="mt-1 flex items-center gap-2 text-sm text-surface-foreground/80">
											<BookOpen className="h-4 w-4" />
											<span>Đọc đến chương {data.user_last_read_chapter}</span>
										</div>
									)}
								</div>
							</div>

							<div className="relative flex-1">
								<Search className="absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-surface-foreground/60" />
								<input
									type="text"
									placeholder="Tìm kiếm chương theo số hoặc tiêu đề..."
									value={searchQuery}
									onChange={(e) => {
										setSearchQuery(e.target.value);
										setCurrentPage(1);
									}}
									className="w-full rounded-2xl border border-surface-muted/60 bg-surface px-12 py-3 text-sm text-primary-foreground placeholder:text-surface-foreground/60 focus:border-primary/50 focus:outline-none focus:ring-2 focus:ring-primary/20"
								/>
							</div>

							<button
								onClick={toggleSortOrder}
								className="flex items-center gap-2 rounded-2xl border border-surface-muted/60 bg-surface px-4 py-3 text-sm font-semibold text-primary-foreground transition hover:bg-primary/10"
								title={sortOrder === "desc" ? "Sắp xếp từ bé đến lớn" : "Sắp xếp từ lớn đến bé"}
							>
								<ArrowUpDown className="h-5 w-5" />
								<span className="hidden sm:inline">
									{sortOrder === "desc" ? "Mới nhất" : "Cũ nhất"}
								</span>
							</button>
						</div>

						{filteredChapters.length === 0 ? (
							<div className="rounded-3xl border border-surface-muted/60 bg-surface/80 p-12 text-center">
								<Search className="mx-auto mb-4 h-12 w-12 text-surface-foreground/40" />
								<p className="text-lg font-semibold text-primary-foreground">Không tìm thấy chương nào</p>
								<p className="mt-2 text-sm text-surface-foreground/70">
									Thử tìm kiếm với từ khóa khác
								</p>
							</div>
						) : (
							<>
								<div className="mb-6 flex items-center justify-between">
									<p className="text-sm text-surface-foreground/70">
										Hiển thị {(currentPage - 1) * chaptersPerPage + 1} -{" "}
										{Math.min(currentPage * chaptersPerPage, filteredChapters.length)} trong tổng số{" "}
										{filteredChapters.length} chương
									</p>
								</div>

								<div className="grid gap-3">
									{paginatedChapters.map((chapter) => (
										<ChapterListItem
											key={chapter.id}
											chapter={chapter}
											slug={slug}
											isLastRead={chapter.chapter === data?.user_last_read_chapter}
										/>
									))}
								</div>

								{totalPages > 1 && (
									<div className="mt-8 flex items-center justify-center gap-2">
										<button
											onClick={() => handlePageChange(currentPage - 1)}
											disabled={currentPage === 1}
											className="rounded-full border border-surface-muted/60 bg-surface px-4 py-2 text-sm font-semibold text-primary-foreground transition hover:bg-primary/10 disabled:cursor-not-allowed disabled:opacity-50"
										>
											Trang trước
										</button>

										<div className="flex gap-2">
											{Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
												let pageNum: number;

												if (totalPages <= 5) {
													pageNum = i + 1;
												} else if (currentPage <= 3) {
													pageNum = i + 1;
												} else if (currentPage >= totalPages - 2) {
													pageNum = totalPages - 4 + i;
												} else {
													pageNum = currentPage - 2 + i;
												}

												return (
													<button
														key={pageNum}
														onClick={() => handlePageChange(pageNum)}
														className={`h-10 w-10 rounded-full border text-sm font-semibold transition ${
															currentPage === pageNum
																? "border-primary bg-primary text-primary-foreground"
																: "border-surface-muted/60 bg-surface text-primary-foreground hover:bg-primary/10"
														}`}
													>
														{pageNum}
													</button>
												);
											})}
										</div>

										<button
											onClick={() => handlePageChange(currentPage + 1)}
											disabled={currentPage === totalPages}
											className="rounded-full border border-surface-muted/60 bg-surface px-4 py-2 text-sm font-semibold text-primary-foreground transition hover:bg-primary/10 disabled:cursor-not-allowed disabled:opacity-50"
										>
											Trang sau
										</button>
									</div>
								)}
							</>
						)}
					</>
				)}
			</div>
		</div>
	);
};

interface ChapterListItemProps {
	chapter: ComicChapterListItem;
	slug: string;
	isLastRead: boolean;
}

const ChapterListItem = ({ chapter, slug, isLastRead }: ChapterListItemProps) => {
	return (
		<Link
			href={`/user/comic/${slug}/chapter/${chapter.chapter}`}
			className={`group flex items-center justify-between gap-4 rounded-2xl border bg-surface px-5 py-4 transition hover:shadow-md ${
				isLastRead
					? "border-primary/60 bg-primary/5 hover:border-primary/80"
					: "border-surface-muted/60 hover:border-primary/40 hover:bg-primary/5"
			}`}
		>
			<div className="flex flex-1 items-center gap-4">
				<div className="flex items-center gap-2">
					{isLastRead && <BookOpen className="h-5 w-5 text-primary" />}
					<span
						className={`text-base font-bold ${isLastRead ? "text-primary" : "text-primary-foreground"}`}
					>
						{chapter.chapter}
					</span>
				</div>

				<div className="flex flex-1 flex-col gap-1">
					<span className="text-sm font-semibold text-primary-foreground group-hover:text-primary">
						{chapter.title}
					</span>
					<span className="text-xs text-surface-foreground/60">
						Cập nhật {formatRelativeTime(chapter.updated_at)}
					</span>
				</div>
			</div>

			<div className="flex items-center gap-3">
				{chapter.is_locked ? (
					chapter.is_unlocked ? (
						<div className="flex items-center gap-1.5 rounded-full border border-sky-500/50 bg-sky-500/10 px-3 py-1.5 text-xs font-semibold text-sky-600">
							<CheckCircle2 className="h-3.5 w-3.5" />
							<span>Đã mở khóa</span>
						</div>
					) : (
						<div className="flex items-center gap-1.5 rounded-full border border-amber-500/50 bg-amber-500/10 px-3 py-1.5 text-xs font-semibold text-amber-600">
							<Lock className="h-3.5 w-3.5" />
							<span>{chapter.key_require} chìa khóa</span>
						</div>
					)
				) : (
					<div className="flex items-center gap-1.5 rounded-full border border-emerald-500/50 bg-emerald-500/10 px-3 py-1.5 text-xs font-semibold text-emerald-600">
						<CheckCircle2 className="h-3.5 w-3.5" />
						<span>Miễn phí</span>
					</div>
				)}
			</div>
		</Link>
	);
};

const LoadingSkeleton = () => {
	return (
		<>
			<div className="mb-8 flex flex-col gap-6 rounded-3xl border border-surface-muted/60 bg-surface/80 p-6 lg:flex-row lg:items-center">
				<div className="flex items-center gap-4">
					<div className="h-32 w-24 animate-pulse rounded-2xl bg-surface-muted/40" />
					<div className="flex flex-col gap-2">
						<div className="h-7 w-48 animate-pulse rounded bg-surface-muted/40" />
						<div className="h-5 w-32 animate-pulse rounded bg-surface-muted/40" />
						<div className="h-5 w-40 animate-pulse rounded bg-surface-muted/40" />
					</div>
				</div>
				<div className="h-12 flex-1 animate-pulse rounded-2xl bg-surface-muted/40" />
			</div>

			<div className="grid gap-3">
				{Array.from({ length: 10 }).map((_, index) => (
					<div key={index} className="h-20 animate-pulse rounded-2xl bg-surface-muted/40" />
				))}
			</div>
		</>
	);
};

export default ComicChaptersListPage;
