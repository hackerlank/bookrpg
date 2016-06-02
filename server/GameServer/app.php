<?php

define('ROOT', __DIR__ . '/');

$dir = [
    'test/',
    'test/book/rpg/',
    'bookexcel/',
    'bookexcel/template/PHP/libs/',
];

function autoloadClass($name)
{
    global $dir;

    if($name == 'ConfigItemBase'){
        echo $name;
    }

    if (false !== $pos = strrpos($name,'\\')){
        $name = substr($name, $pos + 1);
    }

    foreach ($dir as $val) {

        $file = ROOT . $val . $name . '.php';
        if (file_exists($file)) {
            require_once $file;
            return true;
        }
    }

    return false;
}

// spl_autoload_register('autoloadClass', true, true);

// $st = new \book\rpg\SheetConfMgr();
// $st->init(file_get_contents('test/Sheet.json'), 'json');

// $i1 = $st->getItem(1, '武器商');
// $i3 = $st->getItemByDesc('武器商');

//  $ct = new ConfigCfgMgr();
//  $ct->init(file_get_contents('test/config.txt'));


$test = "\\test"; // outputs \test;

// WON'T WORK: pattern in double-quotes double-escaped backslash
//其实变成了匹配单字符\t
echo preg_replace("~\\\t~", '', $test),PHP_EOL; #output -> \test

// WORKS: pattern in double-quotes with triple-escaped backslash
echo preg_replace("~\\\\t~", '', $test),PHP_EOL; #output -> est

// WON'T WORK: pattern in single-quotes with double-escaped backslash
//其实变成了匹配单字符\t
echo preg_replace('~\\t~', '', $test),PHP_EOL; #output -> \test

// WORKS: pattern in single-quotes with double-escaped backslash
echo preg_replace('~\\\t~', '', $test),PHP_EOL; #output -> est

// WON'T WORK: pattern in single-quotes with double-escaped backslash
//其实变成了匹配单字符\t
echo preg_replace('~[\\]t~', '', $test),PHP_EOL; #output -> \test

// WORKS: pattern in double-quotes with double-escaped backslash inside a character class
echo preg_replace("~[\\\]t~", '', $test),PHP_EOL; #output -> est

// WORKS: pattern in single-quotes with double-escaped backslash inside a character class
echo preg_replace('~[\\\]t~', '', $test),PHP_EOL; #output -> est


echo PHP_EOL;


