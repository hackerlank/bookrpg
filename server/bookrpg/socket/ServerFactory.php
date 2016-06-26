<?php
namespace bookrpg\socket;

class ServerFactory
{
    public static function getInstance($impl = 'Swoole', $config=null)
    {
        $className = __NAMESPACE__ . "\\impl\\" . ucfirst($impl) . 'Server';
        return new $className($config);
    }
}