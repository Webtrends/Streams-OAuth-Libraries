
import base64
import hashlib
import http.client
import hmac
import json
import time
import urllib.error
import urllib.parse
import urllib.request


class token_request:
    audience = "auth.webtrends.com"
    scope = "sapi.webtrends.com"
    auth_url = "https://sauth.webtrends.com/v1/token"
    expiration = ""

    client_id = ""
    client_secret = ""

    def __init__(self, client_id, client_secret):
        self.client_id = client_id
        self.client_secret = client_secret
        self.expiration = time.time() + 30*60

    def execute(self):
        encodedHeader = base64.b64encode(str.encode(json.dumps({"typ": "JWT", "alg": "HS256"})))
        encodedClaims = base64.b64encode(str.encode(json.dumps({"iss": self.client_id, "prn": self.client_id, "aud": self.audience, "exp": self.expiration, "scope": self.scope})))
        sig = base64.b64encode(hmac.new(str.encode(self.client_secret), msg=(encodedHeader+b'.'+encodedClaims), digestmod=hashlib.sha256).digest())
        params = urllib.parse.urlencode({"client_id": self.client_id, "client_assertion": encodedHeader+b'.'+encodedClaims+b'.'+sig})
        up = urllib.parse.urlparse(self.auth_url)
        conn = http.client.HTTPSConnection(up.hostname, 443)
        conn.request("POST", up.path, params, {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"})
        response = conn.getresponse()

        if response.status != 200 or response == "":
            raise Exception("Request error")

        obj = json.loads(response.read().decode('utf-8'))
        return obj["access_token"]
