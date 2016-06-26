<?php
namespace bookrpg\route\impl;

use bookrpg\route\RouteBase;
use bookrpg\route\IMessage;
use bookrpg\core\Facade;

class DefaultRoute extends RouteBase
{
	private $parsers = [];
	private $headParser;

	public function __construct()
	{
		$this->headParser = new DefaultMessage();
	}

	/**
	 * @param mixed $opcode
	 * @param class name $parser
	 */
	public function addMessageParser($opcode, $parser)
	{
		$this->parsers[$opcode] = $parser;
	}

	public function addMessageParserArray($array)
	{
		$this->parsers = array_merge($this->parsers, $array);
	}

    public function buildMessage($data, $sender=null)
    {
    	if (!$this->headParser->parseHead($data)) {
    		return false;
    	}

    	$opcode = $this->headParser->getOpcode();

    	if (!empty($this->parsers[$opcode])) {
    		$class = $this->parsers[$opcode];
    		$message = new $class();
    		$message->parse($data);
			$message->setSender($sender);
    		return $message;
    	} else {
    		Facade::$log->warning('unknown opcode: ' . $opcode);
    	}

    	return false;
    }
}