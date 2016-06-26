<?php

const ROOT = __DIR__ . '/';

include ROOT . '../vendor/autoload.php';

use bookrpg\core\Facade;
use bookrpg\log\LogFactory;
use bookrpg\cache\CacheFactory;
use bookrpg\route\RouteFactory;
use bookrpg\socket\ServerFactory;
use bookrpg\util\Util;

const EOL = "\r\n";
const DS = '/';

Util::include(ROOT);

echo 'Init GameServer' . PHP_EOL;

$config = require ROOT . 'config/config.php';
Facade::$config = $config;

echo 'Init log, type: ' . $config['log_type'] . PHP_EOL;
Facade::$log = LogFactory::getInstance($config['log_type'], $config);

echo 'Init cache, type: ' . $config['cache_type'] . PHP_EOL;
Facade::$cache = CacheFactory::getInstance($config['cache_type'], $config);

echo 'Init route, type: ' . $config['route_type'] . PHP_EOL;
Facade::$route = RouteFactory::getInstance($config['route_type'], $config);

echo 'Init server, type: ' . $config['server_type'] . PHP_EOL;
$server = ServerFactory::getInstance($config['server_type'], $config);
Facade::$server = $server;

$server->onStart()->addListener(function(){
    Facade::$route->addMessageParser(1, '\Login_c2s');
    Facade::$route->addMessageParser(2, '\Login_s2c');
    Facade::$route->addListener(1, function($msg){

    	echo $msg->getUsername() . '  ' . $msg->getPassword() . PHP_EOL;

        $cli = $msg->getSender();

    	$msg = new \Login_s2c();
        $msg->opcode = 2;
    	$msg->setRetCode(0);

    	$cli->send($msg);

    });
});

$server->onReceive()->addListener(function($serv, $cli, $data){
    $message = Facade::$route->buildMessage($data, $cli);
    if($message){
        Facade::$route->dispatch($message);
    } else {
        Facade::$log->warning('received unknown message, ip: ' . $cli->clientIP());
    }
});


$server->start();

