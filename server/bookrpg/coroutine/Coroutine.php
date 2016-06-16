<?php
namespace bookrpg\coroutine;

use bookrpg\coroutine\impl\CoroutineImpl;

class Coroutine
{
    private static $impl;

    private static function getImpl()
    {
        return self::$impl ?: self::$impl = new CoroutineImpl();
    }

    public static function init(ICoroutine $impl)
    {
        self::$impl = $impl;
    }

    public static function start($routine)
    {
        self::getImpl()->start($routine);
    }

    public static function stop($routine)
    {
        self::getImpl()->stop($routine);
    }
}