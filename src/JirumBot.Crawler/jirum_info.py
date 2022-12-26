import datetime


class Article:
    def __init__(self, title, url):
        self.title = title
        self.url = url

    def to_json(self):
        return {
            'title': self.title,
            'url': self.url
        }


class JirumInfo:
    def __init__(self, cool_articles, cool_market_articles, city_articles, quasar_articles, ruli_articles, fm_articles,
                 ppom_articles, clien_articles):
        self.cool_articles = cool_articles
        self.cool_market_articles = cool_market_articles
        self.city_articles = city_articles
        self.quasar_articles = quasar_articles
        self.ruli_articles = ruli_articles
        self.fm_articles = fm_articles
        self.ppom_articles = ppom_articles
        self.clien_articles = clien_articles
        self.timestamp = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')

    def is_empty(self):
        return len(self.cool_articles) == 0 and len(self.cool_market_articles) == 0 and len(
            self.city_articles) == 0 and len(
            self.quasar_articles) == 0 and len(self.ruli_articles) == 0 and len(self.fm_articles) == 0 and len(
            self.ppom_articles) == 0 and len(self.clien_articles) == 0

    def to_json(self):
        return {
            'timestamp': self.timestamp,
            'cool_articles': [a.to_json() for a in self.cool_articles],
            'cool_market_articles': [a.to_json() for a in self.cool_market_articles],
            'city_articles': [a.to_json() for a in self.city_articles],
            'quasar_articles': [a.to_json() for a in self.quasar_articles],
            'ruli_articles': [a.to_json() for a in self.ruli_articles],
            'fm_articles': [a.to_json() for a in self.fm_articles],
            'ppom_articles': [a.to_json() for a in self.ppom_articles],
            'clien_articles': [a.to_json() for a in self.clien_articles]
        }

# articles = [Article('title1', 'url1'), Article('title2', 'url2')]
# jirum_info = JirumInfo(articles, articles, articles, articles, articles, articles, articles, articles)
# import json
#
# test_json = json.dumps(jirum_info, default=lambda o: o.__dict__, indent=4)
# print(test_json)
