<?php
namespace bookrpg\socket\impl;

use bookrpg\socket\IServer;
use bookrpg\core\Event;
use bookrpg\route\RouteBase;
use bookrpg\core\Facade;

class SwooleServer implements IServer
{
    private $clients = [];
    private $config;
    private $serv;
    private $running = false;
    private $onStart;
    private $onConnect;
    private $onClose;
    private $onReceive;
    private $onStop;

    const MODE_SINGLE_THREAD = 'singleThread';
    const MODE_MULTI_THREAD = 'multiThread';
    const MODE_MULTI_PROCESS = 'multiProcess';

    const CONFIG = [
        'server_host' => '0.0.0.0',
        'server_port' => 7749,
        'server_work_mode' => 'singleThread',
        'server_worker_num' => 1,
        'server_max_conn' => 0, //默认ulimit -n
        'server_heartbeat_interval' => 30, //seconds
        'server_max_pack_length' => 32 * 1024, //32KB
        'server_log_file' => '',
    ];

    public function __construct(array $config = null)
    {
        if(!extension_loaded('swoole')) {
            throw new \Exception("not found swoole extension");
        }

        $this->onStart = new Event();
        $this->onConnect = new Event();
        $this->onClose = new Event();
        $this->onReceive = new Event();
        $this->onStop = new Event();

        $this->config = (object)($config ? array_merge(self::CONFIG, $config) : self::CONFIG);
        if(!$this->config->server_log_file) {
            $this->config->server_log_file = __DIR__ . '/SwooleServer.log';
        }

        $this->init();
    }

    public function running()
    {
        return $this->running;
    }

    /**
     * 热更新的数据应该在此处初始化
     * @return Event|args:IServer
     */
    public function onStart()
    {
        return $this->onStart;
    }

    /**
     * @return Event|args:IServer, IClient
     */
    public function onConnect()
    {
        return $this->onConnect;
    }

    /**
     * @return Event|args:IServer, IClient
     */
    public function onClose()
    {
        return $this->onClose;
    }

    /**
     * @return Event|args:IServer, IClient, Data
     */
    public function onReceive()
    {
        return $this->onReceive;
    }

    /**
     * @return Event||args:IServer
     */
    public function onStop()
    {
        return $this->onStop;
    }

    public function start()
    {
        $this->running = $this->serv->start();
        if(!$this->running){
            Facade::critical('cannot start server: ' . 
                $this->config->server_host .':'. $this->config->server_port);
        }
        return $this->running;
    }

    public function send($fd, $data)
    {
        $this->serv->send($fd, $data);
    }

    public function close($fd)
    {
        unset($this->clients[$fd]);
        $this->serv->close($fd);
    }

    public function stop()
    {
        $this->running = false;
        $this->serv->shutdown();
    }

    private function init()
    {
        $config = &$this->config;

        if($config->server_work_mode == self::MODE_SINGLE_THREAD){
            $work_mode = SWOOLE_BASE;
        } else{
            $work_mode = SWOOLE_PROCESS;
        }

        $set = [
            'open_length_check' => true,
            'package_length_type' => 'L',
            'package_max_length' => $config->server_max_pack_length,
            'open_cpu_affinity' => true,
            'cpu_affinity_ignore' => array(0,1),
            'buffer_output_size' => 16 * 1024 * 1024,
            'open_length_check' => true,
            'tcp_defer_accept' => 5,
            'daemonize' => 0,
            'log_file' => $config->server_log_file,
            'log_level' => 1,
            'heartbeat_check_interval' => $config->server_heartbeat_interval,
        ];

        if($config->server_worker_num > 0) {
            $set['max_conn'] = $config->server_max_conn;
        }
        if($config->server_worker_num > 0) {
            $set['worker_num'] = $config->server_worker_num;
        }

        $this->serv = $serv = new \swoole_server(
            $config->server_host, 
            $config->server_port, 
            $work_mode, 
            SWOOLE_SOCK_TCP
        );

        $serv->set($set);

        $serv->on('WorkerStart', function($serv){
            $this->onStart->invoke($this);
        });

        $serv->on('Connect', function($serv, $fd, $from_id){
            echo "connected" . PHP_EOL;
            $cli = new SwooleClient($this, $fd);
            $this->clients[$fd] = $cli;
            $this->onConnect->invoke($this, $cli);
        });

        $serv->on('Receive', function($serv, $fd, $from_id, $data){
            echo "Receive" . PHP_EOL;
            $data = substr($data, 4);//去掉4直接包长
            $this->onReceive->invoke($this, $this->clients[$fd], $data);
        });

        $serv->on('Close', function($serv, $fd, $from_id){
            echo "Close" . PHP_EOL;
            $cli = $this->clients[$fd];
            $cli->disconnect();
            unset($this->clients[$fd]);
            $this->onClose->invoke($this, $cli);
        });

        $serv->on('WorkerStop', function($serv, $fd){
            $this->onStop->invoke($this);
        });

        $serv->on('Shutdown', function($serv, $fd){
            $this->running = false;
        });
    }
}
