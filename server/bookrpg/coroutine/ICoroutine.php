<?php

namespace bookrpg\coroutine;

interface ICoroutine
{
    public function start($routine);

    public function stop($routine);
}