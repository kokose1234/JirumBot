import time
import json

import undetected_chromedriver as uc
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys

from jirum_info import *
from sqs_sender import *


def runqa():
    global quasar_url_history
    global start
    articles = []

    driver.get('https://quasarzone.com/bbs/qb_saleinfo')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="frmSearch"]/div/div[3]/div[4]/table/tbody/tr/td[2]/div/div[2]/p/a')

    for h in save:
        articles.append(Article(title=h.find_element(By.XPATH, 'span[1]').text, url=h.get_attribute('href')))

    if start == 0:
        quasar_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in quasar_url_history]
    quasar_url_history = [h.url for h in articles]

    return new_articles


def runpo():
    global ppom_url_history
    global start
    articles = []

    driver.get('https://www.ppomppu.co.kr/zboard/zboard.php?id=ppomppu')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="revolution_main_table"]/tbody/tr/td[3]/table/tbody/tr/td[2]/div/a')
    del save[-8:]

    for h in save:
        articles.append(Article(title=h.find_element(By.XPATH, 'font[1]').text,
                                url=h.get_attribute('href')))

    if start == 0:
        ppom_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in ppom_url_history]
    ppom_url_history = [h.url for h in articles]

    return new_articles


def runru():
    global ruli_url_history
    global start
    articles = []

    driver.get('https://bbs.ruliweb.com/market/board/1020')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="board_list"]/div/div[2]/table/tbody/tr/td[3]/div')

    for h in save:
        articles.append(Article(title=h.find_element(By.CLASS_NAME, "deco").text,
                                url=h.find_element(By.CLASS_NAME, "deco").get_attribute('href')))

    if start == 0:
        ruli_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in ruli_url_history]
    ruli_url_history = [h.url for h in articles]

    return new_articles


def runfm():
    global fm_url_history
    global start
    articles = []

    driver.get('https://www.fmkorea.com/hotdeal')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="bd_1196365581_0"]/div/div[3]/ul/li/div/h3/a')

    for h in save:
        try:
            articles.append(Article(title=h.text.strip(h.find_element(By.XPATH, 'span[1]').text).strip(" "),
                                    url=h.get_attribute('href')))
        except:
            articles.append(Article(title=h.text, url=h.get_attribute('href')))

    if start == 0:
        fm_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in fm_url_history]
    fm_url_history = [h.url for h in articles]

    return new_articles


def runcl():
    global clien_url_history
    global start
    articles = []

    driver.get('https://www.clien.net/service/board/jirum')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="div_content"]/div[9]/div/div/div[2]/span/a[1]')

    for h in save:
        articles.append(Article(title=h.text,
                                url=h.get_attribute('href')))
    if start == 0:
        clien_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in clien_url_history]
    clien_url_history = [h.url for h in articles]

    return new_articles


def runcool():
    global cool_url_history
    global start
    articles = []

    driver.get('https://coolenjoy.net/bbs/jirum')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr/td[2]')
    del save[:1]

    for h in save:
        try:
            articles.append(Article(title=h.text.strip(h.find_element(By.CLASS_NAME, 'cnt_cmt').text).strip("\n"),
                                    url=h.find_element(By.XPATH, 'a[1]').get_attribute('href')))

        except:
            articles.append(Article(title=h.text, url=h.find_element(By.XPATH, 'a[1]').get_attribute('href')))

    if start == 0:
        cool_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in cool_url_history]
    cool_url_history = [h.url for h in articles]

    return new_articles


def runcoolmt():
    global cool_market_url_history
    global start
    articles = []

    driver.get('https://coolenjoy.net/bbs/mart2?sca=%ED%8C%90%EB%A7%A4')
    time.sleep(1)

    driver.find_element(By.NAME, 'mb_id').send_keys('chbe8041')
    driver.find_element(By.NAME, 'mb_password').send_keys('CHbe0305!@' + Keys.ENTER)
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="fboardlist"]/div[1]/table/tbody/tr/td[2]')

    for h in save:
        try:
            articles.append(Article(title=h.text.strip(h.find_element(By.CLASS_NAME, 'cnt_cmt').text).strip("\n"),
                                    url=h.find_element(By.XPATH, 'a[1]').get_attribute('href')))
        except:
            articles.append(Article(title=h.text, url=h.find_element(By.XPATH, 'a[1]').get_attribute('href')))

    if start == 0:
        cool_market_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in cool_market_url_history]
    cool_market_url_history = [h.url for h in articles]

    return new_articles


def runct():
    global city_url_history
    global start
    articles = []

    driver.get('https://www.city.kr/index.php?mid=ln&page=1')
    time.sleep(1)

    save = driver.find_elements(By.XPATH, '//*[@id="bd_16532954_0"]/div[3]/table/tbody/tr/td[5]/a[1]')
    del save[:1]

    for h in save:
        articles.append(Article(title=h.text, url=h.get_attribute('data-viewer')[:-17]))

    if start == 0:
        city_url_history = [h.url for h in articles]
        return []

    new_articles = [h for h in articles if h.url not in city_url_history]
    city_url_history = [h.url for h in articles]

    return new_articles


start = 0

quasar_url_history = []
cool_url_history = []
ruli_url_history = []
fm_url_history = []
ppom_url_history = []
clien_url_history = []
cool_market_url_history = []
city_url_history = []

while True:
    options = uc.ChromeOptions()
    options.add_argument('--incognito')
    options.add_argument('--headless')
    options.add_argument('--disable-gpu')
    options.add_argument('--no-sandbox')
    options.add_argument('--disable-dev-shm-usage')
    options.add_argument('lang=ko_KR')
    driver = uc.Chrome(use_subprocess=True, options=options)

    jirum_info = JirumInfo(cool_articles=runcool(),
                           ruli_articles=runru(),
                           fm_articles=runfm(),
                           ppom_articles=runpo(),
                           clien_articles=runcl(),
                           cool_market_articles=runcoolmt(),
                           city_articles=runct(),
                           quasar_articles=runqa())

    if not jirum_info.is_empty():
        json_data = json.dumps(jirum_info, default=lambda o: o.__dict__, ensure_ascii=False, indent=4)
        send_sqs_message(json_data)

    driver.quit()

    if start == 0:
        start = 1

    time.sleep(120)
