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
        'log_separator' => ' | ',
        'log_slice_time' => 0, //day week 
        'log_slice_size' => 0, //单位mb
    ];

    public function __construct($config)
    {
        $cfg = self::CONFIG;

        if (!empty($config)) {
            array_merge($cfg, $config);
        }

        $this->config = (object)$cfg;
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
            $separator . implode($separator, array_map('\bookrpg\util\Util::toJson', $context));

            $logFile = $this->config->log_dir . DS . $this->config->log_name . '.' . 
            $this->config->log_suffix;

            return file_put_contents($logFile, $str . EOL, FILE_APPEND|LOCK_EX);
        }

        return false;
    }
}