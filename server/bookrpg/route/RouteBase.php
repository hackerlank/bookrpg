<?php
namespace bookrpg\route;

use bookrpg\core\Facade;

class RouteBase
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
    	if(!isset($this->messages[$opcode])) {
    		return;
    	}

    	foreach ($this->messages[$opcode] as &$value) {
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
    abstract public function buildMessage($data);
}