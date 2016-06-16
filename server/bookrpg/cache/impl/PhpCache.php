<?php
namespace bookrpg\cache\impl;

use  bookrpg\cache\ICache;

class PhpCache implements ICache
{
    private $cache = array();

    public function __construct($config = null)
    {
        
    }

    public function enable()
    {
        return true;
    }

    public function selectDb($db)
    {
        return true;
    }

    public function add($key, $value, $timeOut = 0)
    {
        if (isset($this->cache[$key])) {
            throw new \Exception("{$key} exitst");
        }
        $timeOut = $timeOut ? (time() + $timeOut) : 0;
        return $this->cache[$key] = array(
            $value, $timeOut
        );
    }

    public function set($key, $value, $timeOut = 0)
    {
        $timeOut = $timeOut ? (time() + $timeOut) : 0;
        return $this->cache[$key] = array(
            $value, $timeOut
        );
    }

    public function get($key)
    {
        if(empty($this->cache[$key])) {
            return null;
        }

        if(!empty($this->cache[$key][1]) && $this->cache[$key][1] <= time()) { //过期了
            unset($this->cache[$key]); 
            return null;
        }
        return $this->cache[$key][0];
    }

    public function delete($key)
    {
        unset($this->cache[$key]);
        return true;
    }

    public function increment($key, $step = 1)
    {
        if(!empty($this->cache[$key][0])) {
            if (!\is_numeric($this->cache[$key][0])) {
                throw new \Exception("value no numeric");
            }
            $this->cache[$key][0] += $step;
        } else {

            $this->cache[$key] = array(
                $step, 0
            );
        }
        return $this->cache[$key][0];
    }

    public function decrement($key, $step = 1)
    {
        if(!empty($this->cache[$key][0])) {
            if (!\is_numeric($this->cache[$key][0])) {
                throw new \Exception("value no numeric");
            }
            $this->cache[$key][0] -= $step;
        } else {

            $this->cache[$key] = array(
                0-$step, 0
            );
        }
        return $this->cache[$key][0];
    }

    public function clear()
    {
        return $this->cache = array();
    }
}