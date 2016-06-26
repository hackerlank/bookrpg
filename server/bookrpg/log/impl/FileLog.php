<?php
namespace bookrpg\log\impl;

use bookrpg\log\LogBase;
use bookrpg\log\Level;
use bookrpg\util\File;

class FileLog extends LogBase
{
    private $config;

    const CONFIG = [
        'log_name' => 'log',
        'log_level' => Level::ALL,
        'log_dir' => '.',
        'log_suffix' => 'log',
        'log_separator' => ' ',
        'log_slice_size' => 0, //单位mb
    ];

    public function __construct($config=null)
    {
        $this->config = (object)($config ? array_merge(self::CONFIG, $config) : self::CONFIG);
    }

    /**
     * @param $level
     * @param $message
     * @param array $context
     * @return bool is success 
     */
    public function log($level, $message, array $context = [])
    {
        if (Level::$levels[$level] & $this->config->log_level) {
            if(!File::mkdir($this->config->log_dir)){
                return false;
            }

            $separator = $this->config->log_separator;
            $str = date("Y-m-d H:i:s") . $separator . $level . $separator . $message . 
            $separator . implode($separator, array_map('\bookrpg\util\Util::toJson', $context)) . EOL;

            $logFile = $this->config->log_dir . DS . $this->config->log_name . '.' . 
            $this->config->log_suffix;

            echo $str;

            return file_put_contents($logFile, $str, FILE_APPEND|LOCK_EX);
        }

        return false;
    }
}