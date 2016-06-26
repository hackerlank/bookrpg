<?php
namespace bookrpg\log;

class LogFactory
{
	/**
	 * @param  string $impl
	 * @param  Array $config
	 * @return LogBase
	 */
    public static function getInstance($impl = 'File', $config = null)
    {
    	$className = __NAMESPACE__ . "\\impl\\" . ucfirst($impl) . 'Log';
        return new $className($config);
    }
}
