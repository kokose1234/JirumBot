import time
import re
import json
import orjson as json2


import undetected_chromedriver as uc
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from jirum_info import *
from sqs_sender import *


def craw_qa(tab_num):
    global quasarzone_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="frmSearch"]/div/div[3]/div[4]/table/tbody/tr/td[2]/div/div[2]/p/a')

    for i in contents_list:
        contents_url=i.get_attribute('href')
        contents_text=i.find_element(By.XPATH, 'span[1]').text

        articles.append(Article(title=contents_text,
                                url=contents_url))

    if not quasarzone_previous_contents:
        quasarzone_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in quasarzone_previous_contents:
                new_contents.append(i)
            else:
                break

        quasarzone_previous_contents = [i.url for i in articles]

        return new_contents


def craw_po(tab_num):
    global ppom_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="revolution_main_table"]/tbody/tr/td[3]/table/tbody/tr/td[2]/div/a')

    for i in contents_list:
        contents_url = i.get_attribute('href')
        contents_text = i.text

        if 'divpage=75' in contents_url:
            articles.append(Article(title=contents_text,
                                    url=contents_url))

    if not ppom_previous_contents:
        ppom_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in ppom_previous_contents:
                new_contents.append(i)
            else:
                break

        ppom_previous_contents = [i.url for i in articles]

        return new_contents


def craw_ru(tab_num):
    global ruli_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="board_list"]/div/div[2]/table/tbody/tr/td[3]/div/a[@class="deco"]')

    for i in contents_list:
        contents_url=i.get_attribute('href')
        contents_text=i.text

        articles.append(Article(title=contents_text,
                                url=contents_url))

    if not ruli_previous_contents:
        ruli_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in ruli_previous_contents:
                new_contents.append(i)
            else:
                break

        ruli_previous_contents = [i.url for i in articles]

        return new_contents


def craw_fm(tab_num):
    global fm_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="bd_1196365581_0"]/div/table/tbody/tr[not(@*)]/td[2]/a[1]')

    for i in contents_list:
        contents_url = i.get_attribute('href')
        contents_text = i.text

        articles.append(Article(title=contents_text,
                                url=contents_url))

    if not fm_previous_contents:
        fm_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in fm_previous_contents:
                new_contents.append(i)
            else:
                break

        fm_previous_contents = [i.url for i in articles]
        return new_contents


def craw_cl(tab_num):
    global clien_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="div_content"]/div[9]/div/div/div[2]/span/a[1]')

    for i in contents_list:
        contents_url = i.get_attribute('href')
        contents_text = i.text

        articles.append(Article(title=contents_text,
                                    url=contents_url))

    if not clien_previous_contents:
        clien_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in clien_previous_contents:
                new_contents.append(i)
            else:
                break

        clien_previous_contents = [i.url for i in articles]

        return new_contents


def craw_cool(tab_num):
    global cool_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr[@class=""]/td[2]/a[1]')

    for i in contents_list:
        contents_url = i.get_attribute('href')
        contents_text = i.text

        if len(i.find_elements(By.XPATH, 'span')) == 3:
            articles.append(Article(title=contents_text.rstrip(i.find_elements(By.XPATH, 'span')[2].text + '\r\n'),
                                    url=contents_url))
        else:
            articles.append(Article(title=contents_text,
                                    url=contents_url))

    if not cool_previous_contents:
        cool_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in cool_previous_contents:
                new_contents.append(i)
            else:
                break

        cool_previous_contents = [i.url for i in articles]

        return new_contents


def craw_coolmt(tab_num):
    global cool_market_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr/td[2]/a[1]')
    for i in contents_list:
        contents_url = i.get_attribute('href')
        contents_text = i.text

        articles.append(Article(title=contents_text,
                                url=contents_url))

    if not cool_market_previous_contents:
        cool_market_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in cool_market_previous_contents:
                new_contents.append(i)
            else:
                break
        cool_market_previous_contents = [i.url for i in articles]
        return new_contents


def craw_ct(tab_num):
    global city_previous_contents

    articles = []
    new_contents = []

    driver.switch_to.window(driver.window_handles[tab_num])

    contents_list = driver.find_elements(By.XPATH, '//*[@id="bd_16532954_0"]/div[3]/table/tbody/tr[not(@*)]/td[5]/a[1]')

    for i in contents_list:
        contents_url = i.get_attribute('data-viewer').replace('&listStyle=viewer', '')
        contents_text = i.text

        articles.append(Article(title=contents_text,
                                url=contents_url))

    if not city_previous_contents:

        city_previous_contents = [i.url for i in articles]
        return []
    else:
        for i in articles:
            if i.url not in city_previous_contents:
                new_contents.append(i)
            else:
                break

        city_previous_contents = [i.url for i in articles]
        return new_contents


def startcraw(**kwargs):
    tab_num={}

    start1= time.time()
    for key,value,count in zip(kwargs.keys(),kwargs.values(),range(0,len(kwargs)+1)):
        if count == 0:
            driver.get(value)
        else:
            driver.switch_to.new_window('tab')
            driver.get(value)
        if count == len(kwargs)-1:
            driver.switch_to.window(driver.window_handles[0])
            WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.NAME, 'mb_password')))
            driver.find_element(By.NAME, 'mb_id').send_keys('아디')
            driver.find_element(By.NAME, 'mb_password').send_keys('비번' + Keys.ENTER)
            #WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.XPATH, '//*[@id="fboardlist"]/div[1]')))

        tab_num.update({key: count})
    print(time.time()-start1)
    start2= time.time()
    jirum_info = JirumInfo(cool_articles=craw_cool(tab_num["coolenjoy"]),
                           ruli_articles=craw_ru(tab_num["ruliweb"]),
                           fm_articles=craw_fm(tab_num["fmkorea"]),
                           ppom_articles=craw_po(tab_num["ppomppu"]),
                           clien_articles=craw_cl(tab_num["clien"]),
                           cool_market_articles=craw_coolmt(tab_num["coolenjoymt"]),
                           city_articles=craw_ct(tab_num["city"]),
                           quasar_articles=craw_qa(tab_num["quasarzone"])
                            )
    driver.quit()
    print(time.time()-start2)
    if not jirum_info.is_empty():
        json_data = json2.dumps(jirum_info, default=lambda o: o.__dict__,option=json2.OPT_INDENT_2).decode("utf-8")
        print(json_data)
        start2= time.time()
        send_sqs_message(json_data)
        print("send_sqs_message : ", time.time() - start2)


quasarzone_previous_contents = []
cool_previous_contents = []
ruli_previous_contents = []
fm_previous_contents = []
ppom_previous_contents = []
clien_previous_contents = []
cool_market_previous_contents = []
city_previous_contents = []

while True:

    options = uc.ChromeOptions()
    options.add_argument('--incognito') # 시크릿모드
    options.add_argument('--headless') # 창 안띄우기
    options.add_argument('--disable-gpu') # GPU 가속 사용 안함
    options.add_argument('--no-sandbox') # 보안기능 끄기
    options.add_argument('--disable-dev-shm-usage') # 개발자모드 사용 안함
    options.add_argument('lang=ko_KR') # 언어 설정
    options.add_argument('--blink-settings=imagesEnabled=false') # 이미지 로딩 안함
    options.add_argument('--mute-audio') # 음소거 모드
    options.add_argument("--disable-notifications") # 알림창 안띄우기
    options.add_argument("--disable-popup-blocking") # 팝업창 안띄우기
    options.add_argument("--disable-extensions") # 확장기능 사용 안함
    options.add_argument("--disable-default-apps") # 기본앱 사용 안함
    options.add_argument("--disable-features=VizDisplayCompositor") # 화면 깜빡임 방지
    options.add_argument("--disable-features=EnableEphemeralFlashPermission") # 플래시 사용 안함
    options.add_argument("--disable-features=ImprovedCookieControls") # 쿠키 사용 안함
    options.add_argument("--disable-features=SameSiteByDefaultCookies") # 쿠키 사용 안함
    options.add_argument('--no-proxy-server')
    options.page_load_strategy = 'none'


    driver = uc.Chrome(use_subprocess=True, options=options)

    startcraw(coolenjoymt='https://coolenjoy.net/bbs/mart2?sca=%ED%8C%90%EB%A7%A4',
              coolenjoy='https://coolenjoy.net/bbs/jirum',
              quasarzone='https://quasarzone.com/bbs/qb_saleinfo',
              ppomppu='https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu',
              ruliweb='https://bbs.ruliweb.com/market/board/1020',
              fmkorea='https://www.fmkorea.com/index.php?mid=hotdeal&listStyle=list&page=1',
              clien='https://www.clien.net/service/board/jirum',
              city='https://www.city.kr/ln'
              )

    time.sleep(75)
