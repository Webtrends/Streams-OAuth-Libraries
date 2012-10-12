<?php

namespace Webtrends\Client\Authentication;

class TokenRequest
{
	public $audience = "sauth.webtrends.com";
	public $scope = "sapi.webtrends.com";
	public $authUrl = "sauth.webtrends.com/v1/token";
	public $expiration;
	
	private $clientId;
	private $clientSecret;
	
	function __construct($clientId, $clientSecret)
	{
		$expiration = time() + 30*60;
		$this->clientId = $clientId;
		$this->clientSecret = $clientSecret;
	}
	
	function execute()
	{
		$encodedHeader = base64_encode(json_encode(array("typ"=>"JWT","alg"=>"HS256")));
		$encodedClaims = base64_encode(json_encode(array("iss"=>$this->clientId,"prn"=>$this->clientId,"aud"=>$this->aduience,"exp"=>$this->expiration,"scope"=>$this->scope)));
		$sig = base64_encode(hash_hmac("sha256", "$encodedHeader.$encodedClaims", $this->clientSecret, true));
		$fields = array("client_id"=>$this->clientId,"client_assertion"=>"$encodedHeader.$encodedClaims.$sig");
		
		$ch = curl_init();
		curl_setopt($ch, CURLOPT_URL, $this->audience);
		curl_setopt($ch, CURLOPT_POST, count($fields));
		curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($fields));
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
		$raw_result = curl_exec($ch);
		curl_close($ch);

		if (!$raw_result)
			throw new \Exception("Request error");
		
		$result = json_decode($raw_result, true);
		if (array_key_exists("Error", $result))
			throw new \Exception($result["Description"]);
		
		return $result["access_token"];
	}
}


?>