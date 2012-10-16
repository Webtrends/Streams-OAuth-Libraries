<?php

require_once "TokenRequest.php";
use Webtrends\Client\Authentication\TokenRequest;

try {
	$tr = new TokenRequest("195D066921AA4545A8E6230DFA667DCE","93AF483281EA454D9CEE790B5E92F106");
	$token = $tr->execute();
	print "Your token is: $token\n";
} catch (Exception $e)
{
	print  "Caught exception: " .  $e->getMessage() . "\n";
}


?>