# JirumBot
특가 게시글 알림 디스코드 봇




### 준비사항

1. `.NET 7.0` SDK
2.  최신 버전의 `Google Chrome`
3. `Discord Developer` 계정

#### Setting.json 구조

~~~json
{
    "discordBotToken": "디스코드 봇 토큰",
    "discordGuildId" : 디스코드 서버 Id,
    "discordCategoryId": 디스코드 카테고리 Id,
    "discordChannelId" : 디스코드 채널 Id,
    "adminRoleId": 관리자 역할 Id,
 	"coolIdPath": "//input[@id='ol_id']",
  	"coolPasswordPath": "//input[@id='ol_pw']",
  	"coolLoginPath": "//input[@id='ol_submit']",
  	"coolId": "쿨앤 id",
  	"coolPassword": "쿨앤 비번",
  	"coolBasePath": "//tbody/tr/td[2]/a[1]",
 	"coolMarketBasePath": "/html[1]/body[1]/div[4]/div[2]/div[1]/form[1]/div[1]/table[1]/tbody[1]/tr",
 	"coolMarketTitlePath": "td[2]/a[1]",
  	"coolMarketPricePath": "td[3]/font[1]",
  	"quasarBasePath": 	"/html[1]/body[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[1]/form[1]/div[1]/div[3]/div[4]/table[1]/tbody[1]/tr/td[2]/div[1]/div[2]/p[1]",
  	"quasarTitlePath": "a[1]/span[1]",
  	"quasarUrlPath": "a[1]",
  	"quasarStatusPath": "span[1]",
  	"ppomBasePath": "//tbody/tr[contains(@class, 'list')]/td[3]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/a[1]",
  	"ppomTitlePath": "font[1]",
  	"fmBasePath": "//tbody/tr/td[@class='title hotdeal_var8']/a[1]",
  	"clienBasePath": "//body/div[2]/div[2]/div[1]/div[2]/div[9]/div[1]/div[@class='list_item symph_row jirum  ']/div[2]/span[1]/a[1]",
  	"ruliBasePath": "//tbody/tr/td[3]/div[1]/a[@class='deco']",
  	"meecoBasePath": "//tbody/tr[not(@class)]/td[2]",
  	"meecoCategoryPath": "a[1]",
  	"meecoJirumTitlePath": "a[2]/span[1]",
  	"meecoJirumUrlPath": "a[2]",
  	"ghostCheckInterval": 30,
    "refreshInterval": 120
}
~~~
