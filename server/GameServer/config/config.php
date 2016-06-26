<?php

return [
	'log_type' => 'file',
	'log_name' => 'GameServer',
    'log_level' => 0xff,
    'log_dir' => ROOT . '../log',
    'log_slice_size' => 40, //单位mb

    'cache_type' => 'php',
    'route_type' => 'default',

    'server_type' => 'swoole',
    'server_host' => '0.0.0.0',
    'server_port' => 7749,
    'server_work_mode' => 'singleThread',
    'server_worker_num' => 1,
    'server_max_conn' => 6000, 
    'server_heartbeat_interval' => 30, //seconds
    'server_max_pack_length' => 32 * 1024, //32KB
    'server_log_file' => ROOT . '../log/GameServer_Swoole.log',
];