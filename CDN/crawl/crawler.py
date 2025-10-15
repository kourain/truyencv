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
async def crawl_chapter(comic_slug:str,from_chap:int,step:int=10):
    os.makedirs(f"comic-crawled/{comic_slug}",exist_ok=True)
    browser = await launch(headless=True, args=['--no-sandbox'])
    for i in range(from_chap,from_chap+step):
        try:
            # Step 2: Create a new page in the browser
            page = await browser.newPage()
            await page.setCookie(*cookie)
            # Step 3: Navigate to the webpage
            url = f'https://metruyencv.biz/truyen/{comic_slug}/chuong-{i}'
            await page.goto(url)

            # Step 4: Locate the element with id "chapter-content"
            container:ElementHandle = await page.querySelector('#chapter-content') 

            text = await page.evaluate('(element) => element.innerHTML', container)
            open(f"comic-crawled/{comic_slug}/chap-{i}",'w',encoding='utf-8').write(clean_html_content(text))
        except Exception as e:
            print(f"{i} An error occurred: {e}")
    await browser.close()