/*
Webtrends Streams Javascript-only Oauth System
(c) Webtrends 2013

requires:
- jquery
- a socket handler of some sort
		  
Using the query string in site URLs, this script checks for the presence of an access token.
If none is found, it brings up a friendly branded message to send the user to the streams auth page.
On return, it detects the token and allows the app to run. 
If the token is expired or wrong, it notifies the user.
*/

//set client ID and redirect uri.
//done separately from streams_oauth object declatation for ease of changing
var streams_oauth_config = {
	client_id : "CLIENT_ID_HERE",
	redirect_uri :  encodeURIComponent("http://REDIRECT_URI")
}

//create an object to handle the oauth needs of Webtrends streams
var streams_oauth = {	
	//bring along config variables
	client_id: streams_oauth_config.client_id,
	redirect_uri: streams_oauth_config.redirect_uri,
   
  //construct URL to send user to to authorize
  oauth_url : "https://sauth.webtrends.com/v1/oauth/login?client_id=" + streams_oauth_config.client_id +"&redirect_uri=" + streams_oauth_config.redirect_uri + "&scope=sapi.webtrends.com&response_type=token",
        
  //capture query parameters in this object
  query_parameters : (function()
  {
    var result = {};
    if (window.location.search) {
      // split up the query string and store in an associative array
      var params = window.location.search.slice(1).split("&");
      for (var i = 0; i < params.length; i++) {
        var tmp = params[i].split("=");
        result[tmp[0]] = unescape(tmp[1]);
      }
	  }
		
		return result;
  }()),
	    
  //placeholder for access token when received
  access_token: "",
    
	//functions.
  //present user with message to authorize streams
	show_authorization_message: function(msg){
		if(!msg) msg = "Please log in to Webtrends Streams to begin.";
		$('#streams-oauth').remove();
		$('body').prepend('<div id="streams-oauth" class="streams-oauth-block "><div id="streams-oauth-wrap"> <div id="streams-oauth-signin">        <div id="streams-oauth-signin-body" style="display: block;">        <div class="streams-oauth-notice">Authentication Required.</div>            <div class="streams-oauth-notice-details">' + msg + ' </div>            <a href=' + streams_oauth.oauth_url + '><button class="streams-oauth-account" id="streams-oauth-signin-button"><span>Log in</span></button></a>            <a id="streams-auth-deny" href="#"><div class="streams-oauth-deny-button"><span>Cancel</span></div></a>            <div style="clear:both;"></div>        </div>    </div></div></div>');
		$('.streams-oauth-deny-button').click(function(){
			$('#streams-oauth').remove();
		})
	},
	
  authorize_stream: function(){
		//if we see no token, show the message
		if(!streams_oauth.access_token){
			streams_oauth.show_authorization_message();
		}
	}
}

//set auth token for main object
streams_oauth.access_token = streams_oauth.query_parameters.access_token;