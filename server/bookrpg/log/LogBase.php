<?php

namespace bookrpg\log;


abstract class LogBase
{

    public function emergency($message, array $context = [])
    {
        $this->log(Level::EMERGENCY, $message, $context);
    }

    public function alert($message, array $context = [])
    {
        $this->log(Level::ALERT, $message, $context);
    }

    public function critical($message, array $context = [])
    {
        $this->log(Level::CRITICAL, $message, $context);
    }

    public function error($message, array $context = [])
    {
        $this->log(Level::ERROR, $message, $context);
    }

    public function warning($message, array $context = [])
    {
        $this->log(Level::WARNING, $message, $context);
    }

    public function notice($message, array $context = [])
    {
        $this->log(Level::NOTICE, $message, $context);
    }

    public function info($message, array $context = [])
    {
        $this->log(Level::INFO, $message, $context);
    }

    public function debug($message, array $context = [])
    {
        $this->log(Level::DEBUG, $message, $context);
    }

    /**
     * @param $level
     * @param $message
     * @param array $context
     * @return bool
     * @throws \Exception
     * @desc {type} | {timeStamp} |{dateTime} | {$message}
     */
    abstract public function log($level, $message, array $context = []);

}