<?php

namespace bookrpg\socket\impl;

use bookrpg\socket\IServer;
use bookrpg\socket\IClient;

class SwooleClient implements IClient
{
    private $cli;
    private $serv;
    private $conn;
    private $timeout;
    private $connected;

    const MAX_PACK_LENGTH = 128 * 1024;

    public $uid;

    public function __construct(SwooleServer $serv=null, $conn=null)
    {
        $this->serv = $serv;
        $this->conn = $conn;
        $this->connected = $serv !== null;

        if(!$serv){
            // $cli = new 
        }
    }

    /**
     * seconds
     */
    public function getTimeout()
    {
        return $this->timeout;
    }

    /**
     * seconds
     */
    public function setTimeout($value)
    {
        $this->timeout = $value;
    }

    public function clientIP()
    {
        return 'null';
    }

    public function clientPort()
    {
        return 'null';
    }

    public function remoteIP()
    {
        return 'null';
    }

    public function remotePort()
    {
        return 'null';
    }

    /**
     * @return bool
     */
    public function connected()
    {
        return $this->connected;
    }

    public function connect($host, $port)
    {

    }

    public function send($message)
    {
        $data = $message->serialize();
        $len = strlen($data);
        if($len > self::MAX_PACK_LENGTH){
            throw new \Exception("packet too long:" . $len, 1);
        }
        //uint32 大端
        $data = pack('N', $len + 4) . $data;
        $this->serv->send($this->conn, $data);
    }

    public function disconnect()
    {
        $this->connected = false;
    }
    
    public function close()
    {
        if($this->connected){
            $this->connected = false;
            $this->serv->close($this->conn);
        }
    }
}
