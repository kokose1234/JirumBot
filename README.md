# JirumBot
퀘이사존, 쿨엔조이 게시글 알림 디스코드 봇



### 참고

​	봇 종료시에는 디스코드 채팅에 /종료를 입력하여 종료해야함.




### 준비사항

1. `.NET 5.0` SDK
2.  최신 버전의 `Google Chrome`
3. `Discord Developer` 계정
4. `퀘이사존`, `쿨엔조이` 계정



#### Setting.json 구조

~~~json
{
    "discordBotToken": "디스코드 봇 토큰",
    "discordGuildId" : "디스코드 서버 Id",
    "discordChannelId" : "디스코드 채널 Id",
    "coolId": "쿨엔조이 Id",
    "coolPassWord": "쿨엔조이 비밀번호",
    "coolJirumCategoryPath" : "/html[1]/body[1]/div[4]/div[2]/h2[1]",
    "coolJirumTitlePath" : "/html[1]/body[1]/div[4]/div[2]/div[1]/form[1]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/a[1]",
    "coolJirumUrlPath": "/html[1]/body[1]/div[4]/div[2]/div[1]/form[1]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/a[1]",
    "coolJirumTitlePath2" : "/html[1]/body[1]/div[4]/div[2]/div[1]/form[1]/div[1]/table[1]/tbody[1]/tr[3]/td[2]/a[1]",
    "coolJirumUrlPath2": "/html[1]/body[1]/div[4]/div[2]/div[1]/form[1]/div[1]/table[1]/tbody[1]/tr[3]/td[2]/a[1]",
    "quasarId": "퀘이사존 Id",
    "quasarPassWord": "퀘이사존 비밀번호",
    "quasarJirumTitlePath" : "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[2]/div[4]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/div[2]/p[1]/a[1]/span[1]",
    "quasarJirumUrlPath": "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[2]/div[4]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/div[2]/p[1]/a[1]",
    "quasarJirumThumbnailUrlPath": "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[2]/div[4]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/div[1]/a[1]/span[1]",
    "quasarJirumTitlePath2" : "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[3]/div[3]/table[1]/tbody[1]/tr[1]/td[2]/p[1]/a[2]",
    "quasarJirumUrlPath2": "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[3]/div[3]/table[1]/tbody[1]/tr[1]/td[2]/p[1]/a[2]",
    "quasarJirumThumbnailUrlPath2": "/html[1]/body[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/form[1]/div[1]/div[3]/div[3]/table[1]/tbody[1]/tr[1]/td[2]/p[1]/a[1]/span[1]",
   	"keywords": []
}
~~~
