import json,os,re,asyncio
from pyppeteer import launch
from pyppeteer.element_handle import ElementHandle
raw_json = open("metruyencv.biz.cookies.json",'r',encoding='utf-8').read()
js1 = json.loads(raw_json)
# js2 = json.loads(raw_json.replace("metruyencv.biz","metruyencv.com"))

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
    "Thiên Tài" : 4037,
    "Trò Chơi" : 4038,
    "Góc Nhìn Nam" : 5001,
    "Góc Nhìn Nữ" : 5002
}
def clean_html_content(html):
    """
    Loại bỏ tất cả HTML tags và làm sạch text
    Phù hợp cho crawl truyện
    """
    if not html:
        return ""

    # Thay thế <br>, <p> bằng newline
    html = re.sub(r'<br\s*/?>', '\n', html, flags=re.IGNORECASE)
    html = re.sub(r'</p>', '', html, flags=re.IGNORECASE)
    html = re.sub(r'<p[^>]*>', '', html, flags=re.IGNORECASE)
    
    # Loại bỏ tất cả thẻ HTML còn lại
    html = re.sub(r'<[^>]+>', '', html)
    
    # Decode HTML entities
    html_entities = {
        '&nbsp;': ' ',
        '&amp;': '&',
        '&lt;': '<',
        '&gt;': '>',
        '&quot;': '"',
        '&#39;': "'",
        '&ldquo;': '"',
        '&rdquo;': '"',
        '&lsquo;': "'",
        '&rsquo;': "'",
        '&mdash;': '—',
        '&ndash;': '–',
    }
    for entity, char in html_entities.items():
        html = html.replace(entity, char)
    
    # Loại bỏ khoảng trắng thừa nhưng giữ nguyên paragraph breaks
    lines = html.split('\n')
    lines = [' '.join(line.split()) for line in lines]  # Loại bỏ khoảng trắng thừa mỗi dòng
    html = '\n'.join(line for line in lines if line.strip())  # Loại bỏ dòng trống
    # return re.sub(r'ch[ưu][ơo]ng\s+\d+\s*[.:\-\)].*\n?', '', html.strip(), flags=re.MULTILINE | re.IGNORECASE).strip()
    return html.strip()
cookie = json.loads(open("metruyencv.biz.cookies.json",'r',encoding='utf-8').read())
import asyncio
URLS = [
    "https://metruyencv.biz",
    "https://metruyencv.biz"
]
async def scrape(urls:list[str],from_chap:int,step:int=10):
    browser = await launch(headless=False, executablePath='C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe')
    os.makedirs("truyen",exist_ok=True)

    async def run(slug:str,chap:int,url=0):
        slug = slug.strip().replace("#","")
        if os.path.exists(f"truyen/{slug}/chap-{chap}"):
            os.renames(f"truyen/{slug}/chap-{chap}",f"truyen/{slug}/chap-{chap}.txt")
            return
        if os.path.exists(f"truyen/{slug}/chap-{chap}.txt"):
            return
        page = await browser.newPage()
        # page.setDefaultNavigationTimeout(60000)  # 60 giây
        # await page.setCookie(*(cookie[url]))
        await page.setCookie(*js1)
        # Bật request interception
        await page.setRequestInterception(True)
        
        async def block_resources(request):
            # Chặn image, media, font
            if request.resourceType in ['image', 'media']: #, 'font', 'stylesheet']:
                await request.abort()
                return
            else:
                await request.continue_()
        
        page.on('request', lambda req: asyncio.ensure_future(block_resources(req)))
        try:
        # if True:
            # Step 2: Create a new page in the browser
            if chap == 0:
                if not os.path.exists(f"truyen/{slug}/index.json"):
                    await page.goto(f"{URLS[url]}/truyen/{slug}")
                    title = await page.title()
                    # description_div = r'<div class="text-gray-600 dark:text-gray-300 py-4 px-2 md:px-1 text-base break-words" bis_skin_checked="1">(.)*</div>'
                    description_container:ElementHandle | None = await page.querySelector('div.text-gray-600.dark\\:text-gray-300.py-4.px-2.md\\:px-1.text-base.break-words')
                    description_html = await page.evaluate('(element) => element.innerHTML', description_container)
                    # description = re.search(description_div,description_html).group(0)
                    description = clean_html_content(description_html)
                    section:ElementHandle = await page.querySelector('div.mb-4.mx-auto.text-center.md\\:mx-0.md\\:text-left div.leading-10.md\\:leading-normal.space-x-4')
                    span:list[ElementHandle] = await section.querySelectorAll('span')
                    category_ids = []
                    for s in range(span.__len__()):
                        if s > 1:
                            text = await page.evaluate('(element) => element.innerText', span[s])
                            # print(text)
                            category_ids.append(category_dict[text])
                    open(f"truyen/{slug}/index.json",'w',encoding='utf-8').write(json.dumps({
                        "title": title.replace(" - Metruyencv.biz","").replace("Convert","").replace("\n"," ").strip(),
                        "description": description,
                        "author": await page.evaluate('(element) => element.innerText', await page.querySelector("div.mb-4.mx-auto.text-center.md\\:mx-0.md\\:text-left div.mb-6 a.text-gray-500")),
                        "cover_url": await page.evaluate('(element) => element.src', await page.querySelector("img.w-44.h-60.shadow-lg.rounded.mx-auto")),
                        "main_category_id": category_dict[await page.evaluate('(element) => element.innerText', span[1])] or 0,
                        "category_ids": category_ids,
                        "embedded_from": URLS[url],
                        "embedded_from_url": f"{URLS[url]}/truyen/{slug}",
                        "comic_status": f"{await page.evaluate('(element) => element.innerText', span[0])}"
                    },ensure_ascii=False,indent=4))
                    ads = {}
                    ad_t = await page.querySelector("div#masthead a#topbox-one")
                    if ad_t:
                        slug = (await page.evaluate('(element) => element.href', ad_t)).split('/')[-1]
                        img = await ad_t.querySelector('img')
                        if img:
                            ads[slug] = await page.evaluate('(element) => element.src', img)
                    ad_t = await page.querySelector("div#middle-one a#topbox-two")
                    if ad_t:
                        slug = (await page.evaluate('(element) => element.href', ad_t)).split('/')[-1]
                        img = await ad_t.querySelector('img')
                        if img:
                            ads[slug] = await page.evaluate('(element) => element.src', img)
                    ad_t = await page.querySelector("div#middle-two a#topbox-three")
                    if ad_t:
                        slug = (await page.evaluate('(element) => element.href', ad_t)).split('/')[-1]
                        img = await ad_t.querySelector('img')
                        if img:
                            ads[slug] = await page.evaluate('(element) => element.src', img)
                    open(f"truyen/ads/{slug}.json",'w',encoding='utf-8').write(json.dumps(ads,ensure_ascii=False,indent=4))
                    await page.close()
                    return
                await page.close()
                return
            # Step 3: Navigate to the webpage
            await page.goto(f"{URLS[url]}/truyen/{slug}/chuong-{chap}")
            title = await page.title()
            if title.lower().find("not found") >= 0:
                await page.close()
                return
            # Step 4: Locate the element with id "chapter-content"
            container:ElementHandle | None = await page.querySelector('#chapter-content') 

            text = await page.evaluate('(element) => element.innerHTML', container)
            if text.__len__() < 1000:
                print(f"{chap} Chapter not found or too short.")
                await page.close()
                if url == 0:
                    await run(slug,chap,url=1)
                return
            open(f"truyen/{slug}/chap-{chap}.txt",'w',encoding='utf-8').write(clean_html_content(text))
        except Exception as e:
            print(f"{chap} An error occurred: {e}")
            await page.close()
            if url == 0:
                await run(slug,chap,url=1)
                pass
            return
        await page.close()
    os.makedirs("truyen/ads",exist_ok=True)
    for url in urls:
        url = url.strip()
        print(f"Scraping story: {url}")
        os.makedirs(f"truyen/{url}",exist_ok=True)
        task = []
        for i in range(from_chap,from_chap+step):
            task.append(run(url,i))
            if len(task) == 10:
                print(f"Processing up to chapter {i}...")
                await asyncio.gather(*task)
                task.clear()
        if task.__len__() > 0:
            print(f"Processing remaining chapters up to {from_chap+step}...")
            await asyncio.gather(*task)
    await browser.close()
list_comic = list(json.load(open("metruyencv.biz.crawl_list.json",'r',encoding='utf-8')))
asyncio.run(scrape(list_comic[300:],0,21))