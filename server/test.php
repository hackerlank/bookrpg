<?php


include __DIR__ . '/vendor/autoload.php';

use Recoil\React\ReactKernel;
use Recoil\Recoil;
use bookrpg\coroutine\Coroutine;
use React\EventLoop\StreamSelectLoop;




Coroutine::start(hello());

// $cr = Coroutine::startCoroutines(hello2());
// Coroutine::startCoroutine(hello());
// Coroutine::startCoroutine(hello2());
// 
// 


swoole_timer_tick(100000, function($id){
    echo "swoole".PHP_EOL;
});

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
    $i = 10;
    while ($i-- > 0) {
        yield;
        echo $i .PHP_EOL;
    }
    echo 'world3!' . PHP_EOL;
}


return;

$c  = 0;

function world()
{
	global $time, $c;
	while(time() - $time < 1){
		echo $c++ . PHP_EOL;
    	yield;
	}
    
    echo 'world!' . PHP_EOL;
}

ReactKernel::start(function () {
	global $time;
	$time = time();
    yield hello();
    yield world();
});

function multiply($a, $b)
{
    yield; // force PHP to parse this function as a generator
    return $a * $b;
    echo 'This code is never reached.';
}

ReactKernel::start(function () {
    $result = yield multiply(2, 3);
    echo '2 * 3 is ' . $result . PHP_EOL;
});

function multiply2($a, $b)
{
    if (!is_numeric($a) || !is_numeric($b)) {
        throw new InvalidArgumentException();
    }

    yield; // force PHP to parse this function as a generator
    return $a * $b;
}

ReactKernel::start(function() {
    try {
        yield multiply2(1, 'foo');
    } catch (InvalidArgumentException $e) {
        echo 'Invalid argument!';
    }
});