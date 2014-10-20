var webtrends = require('./webtrends-auth');

var clientId = ''; // TODO: input your Webtrends clientId here
var clientSecret = ''; // TODO: input your Webtrends client secret here

webtrends.tokenRequest(clientId, clientSecret, function(error, token){
  if(error) {
    console.log("Failed to obtain a authentication token from Webtrends: "+error);
  }
  else {
    console.log("Your Webtrends authentication token is :"+token);
  }
});
