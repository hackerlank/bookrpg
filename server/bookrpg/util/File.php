<?php
namespace bookrpg\util;

class File
{
	/**
     * 递归创建目录
     * @param $dir
     * @param int $mode
     * @return bool
     */
    public static function mkdir($dir, $mode = 0755)
    {
        if (is_dir($dir) || mkdir($dir, $mode, true)) {
            return true;
        }
        if (!self::makedir(dirname($dir), $mode)) {
            return false;
        }
        return mkdir($dir, $mode);
    }

    /**
     * 递归获取目录下的文件
     * @param $dir
     * @param string $filter
     * @param array $result
     * @param bool $deep
     * @return array
     */
    public static function tree($dir, $filter = '', &$result = array(), $deep = false)
    {
        $files = new \DirectoryIterator($dir);
        foreach ($files as $file) {
            if ($file->isDot()) {
                continue;
            }

            $filename = $file->getFilename();

            if ($file->isDir()) {
                self::tree($dir . DS . $filename, $filter, $result, $deep);
            } else {
                if(!empty($filter) && !preg_match($filter,$filename)){
                    continue;
                }
                if ($deep) {
                    $result[$dir] = $filename;
                } else {
                    $result[] = $dir . DS . $filename;
                }
            }
        }
        return $result;
    }

    /**
     * 递归删除目录
     * @param $dir
     * @param $filter
     * @return bool
     */
    public static function rmdir($dir, $filter = '')
    {
        $files = new \DirectoryIterator($dir);
        foreach ($files as $file) {
            if ($file->isDot()) {
                continue;
            }
            $filename = $file->getFilename();
            if (!empty($filter) && !preg_match($filter, $filename)) {
                continue;
            }
            if ($file->isDir()) {
                self::rmdir($dir . DS . $filename);
            } else {
                unlink($dir . DS . $filename);
            }
        }
        return rmdir($dir);
    }
}

