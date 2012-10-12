<?php

require_once "TokenRequest.php";
use Webtrends\Client\Authentication\TokenRequest;

try {
	$tr = new TokenRequest("a","b");
	$token = $tr->execute();
	print "Your token is: $token\n";
} catch (Exception $e)
{
	print  "Caught exception: " .  $e->getMessage() . "\n";
}


?>