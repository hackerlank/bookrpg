<?php


include __DIR__ . '/vendor/autoload.php';

use Recoil\React\ReactKernel;
use bookrpg\coroutine\Coroutine;
use bookrpg\log\LogFactory;







return;


$i = 1000;

$c = new Coroutine();

$c->start(task3());

function task3()
{
    return task1();
}


function task1(){
    global $i;
    echo "wait start" . PHP_EOL;
    while ($i-- > 0) {
        yield;
    }
    echo "wait end" . PHP_EOL;
}

function task2(){
    echo "Hello " . PHP_EOL;
    yield;
    echo "world!" . PHP_EOL;
}


// Coroutine::start(hello());


function hello()
{
    echo 'Hello ' . PHP_EOL;
    yield hello2();
    echo 'world!' . PHP_EOL;
}

function hello2()
{
    echo 'Hello2 ' . PHP_EOL;
    yield hello3();
    echo 'world2!' . PHP_EOL;
}

function hello3()
{
    echo 'Hello3 ' . PHP_EOL;
    echo 'world3!' . PHP_EOL . PHP_EOL;
}
