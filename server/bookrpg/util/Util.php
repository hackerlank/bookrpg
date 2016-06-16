<?php

namespace bookrpg\util;

/**
* 
*/
class Util
{
	public static function toJson($data)
    {
		return json_encode($data,  JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
	}

	public static function fromJson($data)
	{
		return json_decode($data, true);
	}
}

