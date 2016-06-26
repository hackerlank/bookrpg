<?php
namespace bookrpg\coroutine\impl;

use bookrpg\coroutine\ICoroutine;

class CoroutineImpl implements ICoroutine
{
    //ms
    private $tickInterval = 5;

    private $routineList;

    private $tickId = -1;

	public function __construct($config = null)
    {
        if(isset($config['coroutine_tick_interval'])) {
            $this->tickInterval = intval($config['coroutine_tick_interval']);
        }
        $this->routineList = [];
    }

    public function start($routine)
    {
        if(!($routine instanceof \Generator)) {
            $routine = call_user_func($routine);
        }

        $task = new Task($routine);
        $this->routineList[] = $task;
        $this->startTick();
    }

    public function stop($routine)
    {
    	foreach ($this->routineList as $k => $task) {
            if($task->getRoutine() == $routine){
                unset($this->routineList[$k]);
            }
        }
    }

    private function startTick()
    {
        swoole_timer_tick($this->tickInterval, function($timerId){
            $this->tickId = $timerId;
            $this->run();
        });
    }

    private function stopTick()
    {
        if($this->tickId >= 0) {
            swoole_timer_clear($this->tickId);
        }
    }

    private function run()
    {
        if(empty($this->routineList)){
            $this->stopTick();
            return;
        }

        foreach ($this->routineList as $k => $task) {
            $task->run();

            if($task->isFinished()){
                unset($this->routineList[$k]);
            }
        }
    }
    
}