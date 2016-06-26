<?php
namespace bookrpg\cache;

class CacheFactory
{
    /**
     * @param string $impl
     * @param null $config
     * @return ICache
     */
    public static function getInstance($impl = 'Redis', $config = null)
    {
        $className = __NAMESPACE__ . "\\impl\\" . ucfirst($impl) . 'Cache';
        return new $className($config);
    }
}
