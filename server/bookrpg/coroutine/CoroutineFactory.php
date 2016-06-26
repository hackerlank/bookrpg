<?php
namespace bookrpg\coroutine;

use bookrpg\coroutine\ICoroutine;

class CoroutineFactory
{
    /**
     * @param string $impl
     * @param null $config
     * @return ICoroutine
     */
    public static function getInstance($impl = 'Default', $config = null)
    {
        $className = __NAMESPACE__ . "\\impl\\" . ucfirst($impl) . 'Coroutine';
        return new $className($config);
    }
}
