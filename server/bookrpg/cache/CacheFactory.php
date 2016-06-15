<?php
namespace bookrpg;

class CacheFactory
{
    /**
     * @param string $impl
     * @param null $config
     * @return ICache
     * @throws \Exception
     */
    public static function getInstance($impl = 'Redis', $config = null)
    {
    	if($impl == 'Redis') {
            return new RedisCache($config);
        } elseif($impl == 'Yac') {
            return new YacCache($config);
        } else
        {
            return new PhpCache($config);
        }
    }
}
