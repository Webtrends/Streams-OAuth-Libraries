<?php

# requires php5-curl

function wt_oauth_request($clientId, $secret, $url, $scope="sapi.webtrends.com")
{
	$exp = time() + 86400;
	$encodedHeader = base64_encode(json_encode(array("typ"=>"JWT","alg"=>"HS256")));
	$encodedClaims = base64_encode(json_encode(array("iss"=>$clientId,"prn"=>$clientId,"aud"=>"auth.webtrends.com","exp"=>$exp,"scope"=>$scope)));
	$sig = base64_encode(hash_hmac("sha256", "$encodedHeader.$encodedClaims", $secret, true));
	$fields = array("client_id"=>$clientId,"client_assertion"=>"$encodedHeader.$encodedClaims.$sig");

	$ch = curl_init();
	curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_POST, count($fields));
	curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($fields));
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
	$result = json_decode(curl_exec($ch), true);
	curl_close($ch);

	return $result;
}




$clientID = "";
$secretKey = "";

$response = wt_oauth_request($clientId, $secretKey, "http://hcam01.staging.dmz:8081/auth/api");

if (array_key_exists("Error", $response))
{
	print "error: $response[Description]\n";
} else
{
	print "success: \n";
	foreach (array("access_token","token_type","expires_in") AS $k)
		print " * $k: $response[$k]\n";
}


?>