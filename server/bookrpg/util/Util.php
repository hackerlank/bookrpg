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

	public static function include($includeDir, $recursive=true)
    {
        foreach (glob($includeDir . '/*.php') as $file) {
        	@include_once $file;
        }
        
        if ($recursive) {
	        foreach (glob($includeDir .'/*', GLOB_ONLYDIR|GLOB_NOSORT) as $dir)
	        {
	            self::include($dir, $recursive);
	        }
	    }
    }
}

