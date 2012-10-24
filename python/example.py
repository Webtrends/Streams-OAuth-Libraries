
from webtrends.client.authentication.token_request import *

# TODO: Enter your client credentials
client_id = ".."
client_secret = ".."

try:
	tr = token_request(client_id, client_secret)
	token = tr.execute()
	print "Your token is: %s" % token;
except Exception, e:
	print "error: %s" % e

