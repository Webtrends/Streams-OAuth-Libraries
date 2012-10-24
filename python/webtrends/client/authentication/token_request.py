
import json
import time
import httplib, urllib, urlparse
import hashlib, hmac, base64

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
		encodedHeader = base64.b64encode(json.dumps({"typ":"JWT", "alg":"HS256"}))
		encodedClaims = base64.b64encode(json.dumps({"iss":self.client_id, "prn":self.client_id, "aud":self.audience, "exp":self.expiration, "scope":self.scope}))
		sig = base64.b64encode(hmac.new(self.client_secret, msg="%s.%s" % (encodedHeader, encodedClaims), digestmod=hashlib.sha256).digest())
		params = urllib.urlencode({"client_id": self.client_id, "client_assertion": "%s.%s.%s" % (encodedHeader, encodedClaims, sig)});
		up = urlparse.urlparse(self.auth_url)
		conn = httplib.HTTPSConnection(up.hostname, 443)
		conn.request("POST", up.path, params, {"Content-type": "application/x-www-form-urlencoded","Accept": "text/plain"})
		response = conn.getresponse()
		
		if response.status != 200 or response == "":
			raise Exception("Request error")
			
		obj = json.loads(response.read())
		return obj["access_token"]