<?php


namespace bookrpg;

include __DIR__ . '/vendor/autoload.php';

use Recoil\React\ReactKernel;
use bookrpg\coroutine\Coroutine;
use bookrpg\log\LogFactory;
use bookrpg\util\ByteArray;
use bookrpg\util\Endian;

/**
* 
*/
class ClassName
{
    
    public $aa = '';
}

$cn = new ClassName();
echo $cn->aa;

return;

// $data = file_get_contents('/Users/llj/Downloads/1.txt');
// 
$byte = new ByteArray();
$byte->setEndian(Endian::BIG_ENDIAN);

$byte->writeUByte(111);
$byte->writeBoolean(true);
$byte->writeUInt16(21);
$byte->writeUInt32(21);
$byte->writeUInt64(21);
$byte->writeFloat(-1.1);
$byte->writeDouble(-1.1);
$byte->writeString('a你好');
$byte->writeBytes('aaa');

$byte->setPosition(0);
echo $byte->readUByte() . PHP_EOL;
echo $byte->readBoolean() . PHP_EOL;
echo $byte->readUInt16() . PHP_EOL;
echo $byte->readUInt32() . PHP_EOL;
echo $byte->readUInt64() . PHP_EOL;
echo $byte->readFloat() . PHP_EOL;
echo $byte->readDouble() . PHP_EOL;
echo $byte->readString() . PHP_EOL;
echo $byte->readBytes(3) . PHP_EOL;

return;

function float_max($mul = 2, $affine = 1) {
    $max = 1; $omax = 0;
    while((string)$max != 'INF') { $omax = $max; $max *= $mul; }

    for($i = 0; $i < $affine; $i++) {
      $pmax = 1; $max = $omax;
      while((string)$max != 'INF') {
        $omax = $max;
        $max += $pmax;
        $pmax *= $mul;
      }
    }
    return $omax;
  }

$data = pack('ccc', 0x61, 0x62, 0x63);

$byte  = new ByteArray($data);
echo $byte->readByte() . PHP_EOL;
echo $byte->readByte() . PHP_EOL;
echo $byte->readByte() . PHP_EOL;
echo $byte->readByte() . PHP_EOL;


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
