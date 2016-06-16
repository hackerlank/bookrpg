<?php
namespace bookrpg\route\impl;

use bookrpg\route\IMessage;

class DefaultMessage implements IMessage
{
	private $opcode;

    private $route1;

    public $route2;

    public $flag;

    public function getOpcode()
    {
    	return $this->opcode;
    }

    public function parseHead($data)
    {
        list($headSize, 
            $this->$opcode,
            $this->route1,
            $this->route2,
            $this->flag) = unpack('vVvvV', $data);
        
        $this->parseBody(substr($data, $headSize));
    }
    
    public function serializeHead()
    {
    	return pack('vVvvV', 12, $this->opcode, 
            $this->route1, $this->route2, $this->flag);
    }

    public function parseBody($data)
    {
    	
    }

    public function serializeBody()
    {
    	
    }

    public function parse($data)
    {
    	
    }

    public function serialize()
    {
    	
    }
}