import json,os,re,asyncio
from pyppeteer import launch
from pyppeteer.element_handle import ElementHandle
cookie = json.loads(open("metruyencv.biz.cookies.json",'r',encoding='utf-8').read())
def clean_html_content(html):
    """
    Loại bỏ tất cả HTML tags và làm sạch text
    Phù hợp cho crawl truyện
    """
    if not html:
        return ""
    # Loại bỏ script, style, comments
    html = re.sub(r'<script[^>]*>.*?</script>', '', html, flags=re.DOTALL | re.IGNORECASE)
    html = re.sub(r'<style[^>]*>.*?</style>', '', html, flags=re.DOTALL | re.IGNORECASE)
    html = re.sub(r'<!--.*?-->', '', html, flags=re.DOTALL)
    
    # Thay thế <br>, <p> bằng newline
    html = re.sub(r'<br\s*/?>', '\n', html, flags=re.IGNORECASE)
    html = re.sub(r'</p>', '\n\n', html, flags=re.IGNORECASE)
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
    return re.sub(r'ch[ưu][ơo]ng\s+\d+\s*[.:\-\)].*\n?', '', html.strip(), flags=re.MULTILINE | re.IGNORECASE).strip()

cookie = json.loads(open("metruyencv.biz.cookies.json",'r',encoding='utf-8').read())
import asyncio
async def scrape(urls:list[str],from_chap:int,step:int=10):
    browser = await launch(headless=True, executablePath='C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe')
    os.makedirs("truyen",exist_ok=True)
    async def run(slug,chap):
        page = await browser.newPage()
        try:
            # Step 2: Create a new page in the browser
            await page.setCookie(*cookie)
            # Step 3: Navigate to the webpage
            await page.goto(f"https://metruyencv.biz/truyen/{slug}/chuong-{chap}")

            # Step 4: Locate the element with id "chapter-content"
            container:ElementHandle = await page.querySelector('#chapter-content') 

            text = await page.evaluate('(element) => element.innerHTML', container)
            if text.__len__() < 1000:
                print(f"{chap} Chapter not found or too short.")
                await page.close()
                return
            open(f"truyen/{slug}/chap-{chap}",'w',encoding='utf-8').write(clean_html_content(text))
        except Exception as e:
            print(f"{chap} An error occurred: {e}")
        await page.close()
    for url in urls:
        print(f"Scraping story: {url}")
        os.makedirs(f"truyen/{url}",exist_ok=True)
        task = []
        for i in range(from_chap,from_chap+step):
            task.append(run(url,i))
            if i % 20 == 0:
                print(f"Processing up to chapter {i}...")
                await asyncio.gather(*task)
                task = []
    await browser.close()
# await scrape([
#     "chi-kiem-tien-khong-noi-tinh-cam-nghe-nghiep-liem-cau-ta-nhat-di",
#     "tu-tien-mot-lan-co-gang-gap-tram-lan-thu-hoach",
#     "no-le-bong-toi"
# ],1,1000)