<?php
namespace bookrpg\route;

use bookrpg\core\Facade;

abstract class RouteBase
{
	private $messages = [];

    public function addListener($opcode, $callable)
    {
    	if(!isset($this->messages[$opcode])) {
    		$this->messages[$opcode] = [];
    	}
    	$this->messages[$opcode][] = $callable;
    }

    public function removeListener($opcode, $callable)
    {
    	if(!isset($this->messages[$opcode])) {
    		return;
    	}

    	$arr = &$this->messages[$opcode];

    	foreach ($arr as $key => &$value) {
    		if($callable == $value) {
    			unset($arr[$key]);
    		}
    	}
    }

    public function dispatch(IMessage $message)
    {
    	if(!isset($this->messages[$message->getOpcode()])) {
    		return;
    	}

    	foreach ($this->messages[$message->getOpcode()] as &$value) {
			try {
				call_user_func($value, $message);
			} catch (\Exception $e){
				Facade::$log->error($e->getMessage());
			}
    	}
    }

    /**
     * @return IMessage
     */
    abstract public function buildMessage($data, $sender=null);

    abstract public function addMessageParser($opcode, $parser);

    abstract public function addMessageParserArray($array);
}