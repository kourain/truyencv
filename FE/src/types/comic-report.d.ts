import { ComicStatus } from "@const/enum/comic-status";
import { ReportStatus } from "@const/enum/report-status";

export interface ComicReportResponse {
	id: string;
	comic_id: string;
	chapter_id: string | null;
	comment_id: string | null;
	reporter_id: string;
	reason: string;
	status: ReportStatus;
	comic_name: string | null;
	comic_status: ComicStatus | null;
	reporter_email: string | null;
	reporter_name: string | null;
	comment_content: string | null;
	comment_is_hidden: boolean | null;
	created_at: string;
	updated_at: string;
}
