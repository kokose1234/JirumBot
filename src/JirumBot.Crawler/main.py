import time
import json
import re

import undetected_chromedriver as uc
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys

from jirum_info import *
from sqs_sender import *


def craw_qa():
    global quasar_last_num

    articles = []

    driver.get('https://quasarzone.com/bbs/qb_saleinfo')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="frmSearch"]/div/div[3]/div[4]/table/tbody/tr/td[2]/div/div[2]/p/a')

    for h in contents_list:
        contents_url=h.get_attribute('href')
        contents_text=h.find_element(By.XPATH, 'span[1]').text

        if quasar_last_num == 0:
            quasar_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > quasar_last_num:
            articles.append(Article(title=contents_text,
                                    url=contents_url))
        else:
            break

    if articles:
        quasar_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_po():
    global ppom_last_num

    articles = []

    driver.get('https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="revolution_main_table"]/tbody/tr/td[3]/table/tbody/tr/td[2]/div/a')

    for h in contents_list:
        contents_url = h.get_attribute('href')
        contents_text = h.text

        if 'divpage=75' in contents_url:
            if ppom_last_num == 0:
                ppom_last_num = int(max(re.findall(r'\d+', contents_url)))
                return []
            elif int(max(re.findall(r'\d+', contents_url))) > ppom_last_num:
                articles.append(Article(title=contents_text,
                                            url=contents_url))
            else:
                break

    if articles:
        ppom_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_ru():
    global ruli_last_num

    articles = []

    driver.get('https://bbs.ruliweb.com/market/board/1020')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="board_list"]/div/div[2]/table/tbody/tr/td[3]/div/a[@class="deco"]')

    for h in contents_list:
        contents_url=h.get_attribute('href')
        contents_text=h.text

        if ruli_last_num == 0:
            ruli_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > ruli_last_num:
            articles.append(Article(title=contents_text,
                                    url=contents_url))
        else:
            break

    if articles:
        ruli_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_fm():
    global fm_last_num

    articles = []

    driver.get('https://www.fmkorea.com/index.php?mid=hotdeal&listStyle=list&page=1')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="bd_1196365581_0"]/div/table/tbody/tr[not(@*)]/td[2]/a[1]')

    for h in contents_list:
        contents_url = h.get_attribute('href')
        contents_text = h.text

        if fm_last_num == 0:
            fm_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > fm_last_num:
            articles.append(Article(title=contents_text,
                                        url=contents_url))
        else:
             break

    if articles:
        fm_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_cl():
    global clien_last_num

    articles = []

    driver.get('https://www.clien.net/service/board/jirum')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="div_content"]/div[9]/div/div/div[2]/span/a[1]')

    for h in contents_list:
        contents_url = h.get_attribute('href')
        contents_text = h.text

        if clien_last_num == 0:
            clien_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > clien_last_num:
            articles.append(Article(title=contents_text,
                                    url=contents_url))
        else:
            break

    if articles:
        clien_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_cool():
    global cool_last_num

    articles = []

    driver.get('https://coolenjoy.net/bbs/jirum')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr[@class=""]/td[2]/a[1]')

    for h in contents_list:
        contents_url = h.get_attribute('href')
        contents_text = h.text

        if cool_last_num == 0:
            cool_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > cool_last_num:
            if len(h.find_elements(By.XPATH, 'span')) == 3:
                articles.append(Article(title=contents_text.rstrip(h.find_elements(By.XPATH, 'span')[2].text + '\r\n'),
                                        url=contents_url))
            else:
                articles.append(Article(title=contents_text,
                                        url=contents_url))
        else:
            break

    if articles:
        cool_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_coolmt():
    global cool_market_last_num

    articles = []

    driver.get('https://coolenjoy.net/bbs/mart2?sca=%ED%8C%90%EB%A7%A4')

    driver.find_element(By.NAME, 'mb_id').send_keys('chbe8041')
    driver.find_element(By.NAME, 'mb_password').send_keys('CHbe0305!@' + Keys.ENTER)
    time.sleep(1)

    contents_list = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr/td[2]/a[1]')

    for h in contents_list:
        contents_url = h.get_attribute('href')
        contents_text = h.text

        if cool_market_last_num == 0:
            cool_market_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > cool_market_last_num:
            articles.append(Article(title=contents_text,
                                    url=contents_url))
        else:
            break

    if articles:
        cool_market_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles


def craw_ct():
    global city_last_num

    articles = []

    driver.get('https://www.city.kr/ln')

    contents_list = driver.find_elements(By.XPATH, '//*[@id="bd_16532954_0"]/div[3]/table/tbody/tr[not(@*)]/td[5]/a')

    for h in contents_list:
        contents_url = h.get_attribute('data-viewer').replace('&listStyle=viewer', '')
        contents_text = h.text

        if city_last_num == 0:
            city_last_num = int(max(re.findall(r'\d+', contents_url)))
            return []
        elif int(max(re.findall(r'\d+', contents_url))) > city_last_num:
            articles.append(Article(title=contents_text,
                                    url=contents_url))
        else:
            break

    if articles:
        city_last_num = int(max(re.findall(r'\d+', articles[0].url)))

    return articles



quasar_last_num = 0
cool_last_num = 0
ruli_last_num = 0
fm_last_num = 0
ppom_last_num = 0
clien_last_num = 0
cool_market_last_num = 0
city_last_num = 0

while True:
    start= time.time()

    options = uc.ChromeOptions()
    #options.add_argument('--incognito') # 시크릿모드
    options.add_argument('--headless') # 창 안띄우기
    options.add_argument('--disable-gpu') # GPU 가속 사용 안함
    options.add_argument('--no-sandbox') # 보안기능 끄기
    options.add_argument('--disable-dev-shm-usage') # 개발자모드 사용 안함
    options.add_argument('--blink-settings=imagesEnabled=false') # 이미지 로딩 안함


    driver = uc.Chrome(use_subprocess=True, options=options)

    jirum_info = JirumInfo(cool_articles=craw_cool(),
                           ruli_articles=craw_ru(),
                           fm_articles=craw_fm(),
                           ppom_articles=craw_po(),
                           clien_articles=craw_cl(),
                           cool_market_articles=craw_coolmt(),
                           city_articles=craw_ct(),
                           quasar_articles=craw_qa()
                            )

    if not jirum_info.is_empty():
        json_data = json.dumps(jirum_info, default=lambda o: o.__dict__, ensure_ascii=False, indent=4)
        print(json_data)
        start2= time.time()
        send_sqs_message(json_data)
        print("send_sqs_message : ", time.time() - start2)
    driver.quit()

    print("회차완료", time.time() - start,"초 소요됨")
    time.sleep(75)
