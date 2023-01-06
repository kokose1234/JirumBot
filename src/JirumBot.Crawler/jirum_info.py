import datetime
import json

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
    def __init__(self, **kwargs):
        self.kwargs = kwargs

    def is_empty(self):

        return not any(self.kwargs.values())

    def to_json(self):
        data = {**self.kwargs,**{'timestamp': datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')}}

        return json.dumps(data, default=lambda o: o.__dict__, indent=4,ensure_ascii=False)
