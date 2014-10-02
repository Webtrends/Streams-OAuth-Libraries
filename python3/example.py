
import traceback
from webtrends.client.authentication.token_request import token_request

# TODO: Enter your client credentials
client_id = ".."
client_secret = ".."

try:
    tr = token_request(client_id, client_secret)
    token = tr.execute()
    print("Your token is: %s" % token)
except Exception as e:
    traceback.print_exc()
    print("error: %s" % e)
