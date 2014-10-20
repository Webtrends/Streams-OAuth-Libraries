var crypto = require('crypto');
var querystring = require('querystring');
var request = require('request');
var util = require('util');


/**
  * Helper function to obtain a token from Webtrends Streams
  *
  * @param clientId Application ID as provided by Webtrends
  * @param clientSecret Application secret as provided by Webtrends
  * @param callback(errror, body) callback that passes in the authentication token
 **/
function tokenRequest(clientId, clientSecret, callback) {
    var challenge = createChallenge(clientId, clientSecret);

    var options = {
        url : "https://sauth.webtrends.com/v1/token",
        method : "POST",
        useQuerystring : true,
        headers : {
            "Content-type": "application/x-www-form-urlencoded",
            "Accept" : "text/plain"
        },
        body : challenge
    };
    request(options, function(error, res, body) {
        if(error) {
            callback(error);
        }
        else if (res.statusCode != 200) {
            error = new Error("We didn't receive a proper token. Status code "+
                res.statusCode+" Response body: "+body);
            callback(error);
        }
        else callback(null, JSON.parse(body).access_token);
    });
}

/**
  * Helper function to create the token challenge
  *
  * @param clientId Application ID as provided by Webtrends
  * @param clientSecret Application secret as provided by Webtrends
 **/
function createChallenge(clientID, clientSecret){
    var payload = {
        prn : clientID,
        scope : "sapi.webtrends.com"
    };

    var options = {
        expiresInMinutes : 30,
        algorithm : "HS256",
        audience : "auth.webtrends.com",
        issuer : clientID
    };

    var token = sign(payload, clientSecret, options);

    return querystring.stringify({
        client_id : clientID,
        client_assertion : token
    });
}

/**
  * Helper function to create the jwt token signature
  *
  * @param payload object containing the content of the webtoken
  * @param secretOrPrivateKey secret string to use when signing the token
  * @param options object containing optional jwt standard fields
                   {expiresInMinutes, audience, issuer, subject}
 **/
function sign(payload, secretOrPrivateKey, options) {
    options = options || {};
    var header = {typ: 'JWT', alg: options.algorithm || 'HS256'};
    iat = Math.floor(Date.now() / 1000);

    if (options.expiresInMinutes) {
        var ms = options.expiresInMinutes * 60;
        payload.exp = iat + ms;
    }
    if (options.audience)
        payload.aud = options.audience;
    if (options.issuer)
        payload.iss = options.issuer;
    if (options.subject)
        payload.sub = options.subject;

    var encodedHeader = new Buffer(JSON.stringify(header)).toString('base64');
    var encodedPayload = new Buffer(JSON.stringify(payload)).toString('base64');
    var claim = util.format('%s.%s',encodedHeader,encodedPayload);
    var signature = crypto.createHmac('sha256', secretOrPrivateKey).update(claim).digest('base64');
    var signed = util.format('%s.%s', claim, signature);

    return signed;
}

// Export the tokenRequest function
exports.tokenRequest = tokenRequest;
