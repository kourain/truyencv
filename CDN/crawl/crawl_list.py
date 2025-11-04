import json,os,re,asyncio
from pyppeteer import launch
from pyppeteer.element_handle import ElementHandle
cookie = json.loads(open("metruyencv.biz.cookies.json",'r',encoding='utf-8').read())
crawl_list = [
    "chi-kiem-tien-khong-noi-tinh-cam-nghe-nghiep-liem-cau-ta-nhat-di",
    "tu-tien-mot-lan-co-gang-gap-tram-lan-thu-hoach",
    "no-le-bong-toi",
    "ba-muoi-tuoi-moi-den-truong-thanh-he-thong",
    "cau-tai-vo-dao-the-gioi-thanh-thanh",
    "cau-tha-tai-ban-dau-thanh-ma-mon-lam-nhan-tai",
    "vot-thi-nhan",
    "he-thong-phu-ta-truong-sinh-ta-chiu-chet-tat-ca-moi-nguoi",
    "vong-du-chi-tu-vong-vo-hiep",
    "tien-dao-phan-cuoi",
    "xuyen-nhanh-cung-dau-ba-tong-het-thay-xeo-di",
    "xich-tam-tuan-thien",
    "tro-lai-1982-lang-chai-nho",
    "a-day-khong-phai-yeu-duong-tro-choi",
    "ai-muon-cung-nguoi-cai-nay-tiet-thanh-mai-ket-hon-sinh-con-a",
    "bat-dau-bi-bat-coc-han-dung-la-gioi-quyen-quy-thai-tu-gia",
    "phan-phai-tieng-long-bi-nu-chinh-nghe-len-ve-sau-noi-dung-cot-truyen-sap",
    "dau-tu-thien-menh-toc-nhan-thuc-luc-cua-ta-la-toan-toc-tong-cong",
    "chu-thien-tu-thien-ha-thu-nhat-bat-dau-nhap-dao",
]
crawl_list.reverse()
set_crawl_list = set(crawl_list)

category_dict = {
    "Tiên Hiệp" : 1001,
    "Huyền Huyễn" : 1002,
    "Khoa Huyễn" : 1003,
    "Võng Du" : 1004,
    "Đô Thị" : 1005,
    "Đồng Nhân" : 1006,
    "Dã Sử" : 1007,
    "Cạnh Kỹ" : 1008,
    "Huyền Nghi" : 1009,
    "Kiếm Hiệp" : 1010,
    "Kỳ Ảo" : 1011,
    "Light Novel" : 1012,

    "Điềm Đạm" : 2001,
    "Nhiệt Huyết" : 2002,
    "Vô Sỉ" : 2003,
    "Thiết Huyết" : 2004,
    "Nhẹ Nhàng" : 2005,
    "Cơ Trí" : 2006,
    "Lãnh Khốc" : 2007,
    "Kiêu Ngạo" : 2008,
    "Ngu Ngốc" : 2009,
    "Giảo Hoạt" : 2010,

    "Đông Phương Huyền Huyễn" : 3001,
    "Dị Thế Đại Lục" : 3002,
    "Vương Triều Tranh Bá" : 3003,
    "Cao Võ Thế Giới" : 3004,
    "Tây Phương Kỳ Huyễn" : 3005,
    "Hiện Đại Ma Pháp" : 3006,
    "Hắc Ám Huyễn Tưởng" : 3007,
    "Lịch Sử Thần Thoại" : 3008,
    "Võ Hiệp Huyễn Tưởng" : 3009,
    "Cổ Võ Tương Lai" : 3010,
    "Tu Chân Văn Minh" : 3011,
    "Huyễn Tưởng Tu Tiên" : 3012,
    "Hiện Đại Tu Chân" : 3013,
    "Thần Thoại Tu Chân" : 3014,
    "Cổ Điển Tiên Hiệp" : 3015,
    "Viễn Cổ Hồng Hoang" : 3016,
    "Đô Thị Sinh Hoạt" : 3017,
    "Đô Thị Dị Năng" : 3018,
    "Thanh Xuân Vườn Trường" : 3019,
    "Ngu Nhạc Minh Tinh" : 3020,
    "Thương Chiến Chức Tràng" : 3021,
    "Giá Không Lịch Sử" : 3022,
    "Lịch Sử Quân Sự" : 3023,
    "Dân Gian Truyền Thuyết" : 3024,
    "Lịch Sử Quan Trường" : 3025,
    "Hư Nghĩ Võng Du" : 3026,
    "Du Hí Dị Giới" : 3027,
    "Điện Tử Cạnh Kỹ" : 3028,
    "Thể Dục Cạnh Kỹ" : 3029,
    "Cổ Võ Cơ Giáp" : 3030,
    "Thế Giới Tương Lai" : 3031,
    "Tinh Tế Văn Minh" : 3032,
    "Tiến Hóa Biến Dị" : 3033,
    "Mạt Thế Nguy Cơ" : 3034,
    "Thời Không Xuyên Toa" : 3035,
    "Quỷ Bí Huyền Nghi" : 3036,
    "Kỳ Diệu Thế Giới" : 3037,
    "Trinh Tham Thôi Lý" : 3038,
    "Thám Hiểm Sinh Tồn" : 3039,
    "Cung Vi Trạch Đấu" : 3040,
    "Kinh Thương Chủng Điền" : 3041,
    "Tiên Lữ Kỳ Duyên" : 3042,
    "Hào Môn Thế Gia" : 3043,
    "Dị Tộc Luyến Tình" : 3044,
    "Ma Pháp Huyễn Tình" : 3045,
    "Tinh Tế Luyến Ca" : 3046,
    "Linh Khí Khôi Phục" : 3047,
    "Chư Thiên Vạn Giới" : 3048,
    "Nguyên Sinh Huyễn Tưởng" : 3049,
    "Yêu Đương Thường Ngày" : 3050,
    "Diễn Sinh Đồng Nhân" : 3051,
    "Cáo Tiếu Thổ Tào" : 3052,

    "Hệ Thống" : 4001,
    "Lão Gia" : 4002,
    "Bàn Thờ" : 4003,
    "Tùy Thân" : 4004,
    "Phàm Nhân" : 4005,
    "Vô Địch" : 4006,
    "Xuyên Qua" : 4007,
    "Nữ Cường" : 4008,
    "Khế Ước" : 4009,
    "Trọng Sinh" : 4010,
    "Hồng Lâu" : 4011,
    "Học Viện" : 4012,
    "Biến Thân" : 4013,
    "Cổ Ngu" : 4014,
    "Chuyển Thế" : 4015,
    "Xuyên Sách" : 4016,
    "Đàn Xuyên" : 4017,
    "Phế Tài" : 4018,
    "Dưỡng Thành" : 4019,
    "Cơm Mềm" : 4020,
    "Vô Hạn" : 4021,
    "Mary Sue" : 4022,
    "Cá Mặn" : 4023,
    "Xây Dựng Thế Lực" : 4024,
    "Xuyên Nhanh" : 4025,
    "Nữ Phụ" : 4026,
    "Vả Mặt" : 4027,
    "Sảng Văn" : 4028,
    "Xuyên Không" : 4029,
    "Ngọt Sủng" : 4030,
    "Ngự Thú" : 4031,
    "Điền Viên" : 4032,
    "Toàn Dân" : 4033,
    "Mỹ Thực" : 4034,
    "Phản Phái" : 4035,
    "Sau Màn" : 4036,
    "Thiên Tài" : 4037
}
import sys
pathToExtension = os.path.abspath("adblock")
async def scrape():
    browser = await launch(headless=False, executablePath='C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe', args=[
        f'--disable-extensions-except={pathToExtension}',
        f'--load-extension={pathToExtension}',
    ])
    try:
        os.makedirs("truyen",exist_ok=True)
        page = await browser.newPage()
        await page.setCookie(*cookie)
        await page.goto("https://metruyencv.biz/danh-sach/truyen-moi")
        for i in range(400):
            await asyncio.sleep(3.5)
            containner:ElementHandle = await page.querySelector("div.grid.grid-cols-1.md\\:grid-cols-2.gap-6.px-4.lg\\:px-0")
            stories = await containner.querySelectorAll("div.flex.space-x-3.pb-6.border-b.border-auto")
            for story in stories:
                class_css = (await story.getProperty("className")).toString()
                if (class_css.find("flex space-x-3 pb-6 border-b border-auto") != -1):
                    slug_elem = await story.querySelector("a.text-title.font-semibold")
                    if slug_elem:
                        set_crawl_list.add((await slug_elem.getProperty("href")).toString().split("/")[-1])
                        print(f"Found story: {(await slug_elem.getProperty('href')).toString().split('/')[-1]}", flush=True,file=sys.stderr)
            buttons = await page.querySelectorAll("button.disabled\\:border-gray-500.text-primary.disabled\\:text-gray-500")
            next_button = None
            print(buttons)
            for button in buttons:
                bind = (await button.evaluate("el => el.getAttribute('data-x-bind')"))
                print((await (await button.getProperty("_x_attributeCleanups")).getProperty("data-x-bind")).toString())
                if bind.toString().find("NextPage") != -1:
                    next_button = button
                    break
            if next_button is None:
                print("Next button not found, ending scrape.", flush=True, file=sys.stderr)
                break
            if next_button and (await next_button.getProperty("disabled")).toString() != "disabled":
                print(f"Navigating to next page... {i*20}", flush=True, file=sys.stderr)
                await next_button.click()
            else:
                print("No more pages to navigate.", flush=True, file=sys.stderr)
                break
    except Exception as e:
        print(f"Error occurred: {e}", flush=True, file=sys.stderr)
    finally:
        await browser.close()
asyncio.run(scrape())
print(f"Total stories to crawl: {list(set_crawl_list)}")
open("metruyencv.biz.crawl_list.json",'w',encoding='utf-8').write(json.dumps(list(set_crawl_list),ensure_ascii=False,indent=4))