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
    	$impl = ucfirst($impl);
    	$className = __NAMESPACE__ . "\\impl\\{$impl}Log";
        return new $className($config);
    }
}
