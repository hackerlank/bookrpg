<?php

namespace bookrpg\coroutine;

use Generator;

interface ICoroutine
{
    public function start(Generator $routine);

    public function stop(Generator $routine);
}