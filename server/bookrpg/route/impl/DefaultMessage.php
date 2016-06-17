<?php
namespace bookrpg\route\impl;

use bookrpg\route\IMessage;

/**
 * headLength(uint16), not include self length
 * opcode(uint32)
 * route1(uint16)
 * route2(uint16)
 * flag(uint32)
 * ...custom head, total size: headLength - 12
 * ...body
 */
class DefaultMessage implements IMessage
{
    private const HEAD_LENGTH = 12;

    private $headLength;

	public $opcode;

    public $route1;

    public $route2;

    public $flag;

    public function getOpcode()
    {
    	return $this->opcode;
    }

    public function parseHead($data)
    {
        $arr = unpack('vk1/Vk2/vk3/vk4/Vk5', $data);

        $this->headLength = $arr['k1'];
        $this->opcode = $arr['k2'];
        $this->route1 = $arr['k3'];
        $this->route2 = $arr['k4'];
        $this->flag = $arr['k5'];
    }
    
    public function serializeHead()
    {
    	return pack('vVvvV', self::HEAD_LENGTH, $this->opcode, 
            $this->route1, $this->route2, $this->flag);
    }

    public function parseBody($data)
    {
    	$this->parseFromString($data);
    }

    public function serializeBody()
    {
    	return $this->serializeToString();
    }

    public function parse($data)
    {
    	parseHead($data);
        if(strlen($data) > $this->headLength + 2){
            $this->parseBody(substr($data, $this->headLength));
        }
    }

    public function serialize()
    {
    	return serializeHead() . serializeBody();
    }

    public function reset()
    {

    }
}