<?php


include __DIR__ . '/vendor/autoload.php';

use Recoil\React\ReactKernel;
use bookrpg\coroutine\Coroutine;
use bookrpg\log\LogFactory;


class ClassName  
{
    
    public $opcode = 1;

    public $route1 =2;

    public $route2 =3;

    public $flag =4;

    public function getOpcode()
    {
        return $this->opcode;
    }

    public function parseHead($data)
    {
        $arr = unpack('vk1/Vk2/vk3/vk4/Vk5', $data);

        $headSize = $arr['k1'];
        $this->opcode = $arr['k2'];
        $this->route1 = $arr['k3'];
        $this->route2 = $arr['k4'];
        $this->flag = $arr['k5'];

        if(strlen($data) > $headSize + 2){
            $this->parseBody(substr($data, $headSize));
        }
    }
    
    public function serializeHead()
    {
        return pack('vVvvV', 12, $this->opcode, 
            $this->route1, $this->route2, $this->flag);
    }
}

$c = new ClassName();

$head = $c->serializeHead();

$c->parseHead($head);

 print_r($c);


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
