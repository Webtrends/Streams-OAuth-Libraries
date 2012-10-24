<?php

require_once "TokenRequest.php";
use Webtrends\Client\Authentication\TokenRequest;

// TODO: Enter your client credentials
$clientId = "..";
$clientSecret = "..";

try {
	$tr = new TokenRequest($clientId, $clientSecret);
	$token = $tr->execute();
	print "Your token is: $token\n";
} catch (Exception $e)
{
	print  "Caught exception: " .  $e->getMessage() . "\n";
}


?>