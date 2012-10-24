
require './TokenRequest.rb'

# TODO: Enter your client credentials
client_id = ".."
client_secret = ".."

begin
  tr = Webtrends::Client::Authentication::TokenRequest.new client_id, client_secret
  token = tr.execute
  puts "Your token is: #{token}"
rescue Exception => e
  puts "error: #{e.message}"
end


