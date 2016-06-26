<?php
namespace bookrpg\core;

/**
* like C#'s Action
*/
class Event
{
	private $handlers = [];
    private $onceHandlers = [];

    /**
     * Add event Listener
     */
    public function addListener(callable $handler)
    {
        if(is_callable($handler) && !in_array($handler, $this->handlers)){
            $this->handlers[] = $handler;
        }
    }

    /**
     * action will be removed after calling once
     */
    public function addOnceListener(callable $handler)
    {
        if(is_callable($handler) && !in_array($handler, $this->onceHandlers)){
            $this->onceHandlers[] = $handler;
        }
    }

    /**
     * trigger event
     */
    public function invoke(...$args)
    {
        foreach ($this->handlers as $handler) {
            call_user_func_array($handler, $args);
        }

        if (!empty($this->onceHandlers)) {
            foreach ($this->onceHandlers as $handler) {
                call_user_func_array($handler, $args);
            }
            $this->onceHandlers = [];
        }
    }

    public function invokeAndRemove(...$args)
    {
        foreach ($this->handlers as $handler) {
            call_user_func_array($handler, $args);
        }

        foreach ($this->onceHandlers as $handler) {
            call_user_func_array($handler, $args);
        }

        $this->handler = [];
        $this->onceHandlers = [];
    }
    /**
     * remove event Listener
     */
    public function removeListener(callable $handler)
    {
        foreach ($this->handlers as $key => $value) {
            if ($handler == $value) {
                unset($this->handlers[$key]);
                break;
            }
        }

        foreach ($this->onceHandlers as $key => $value) {
            if ($handler == $value) {
                unset($this->onceHandlers[$key]);
                break;
            }
        }
    }

    public function removeAllListeners()
    {
        $this->handlers = [];
        $this->onceHandlers = [];
    }
}
