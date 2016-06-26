<?php
namespace bookrpg\socket;

interface IServer
{
	public function start();

    public function stop();

 	/**
     * @return bool
     */
    public function running();

    /**
     * @return Event
     */
    public function onStart();

 	/**
     * @return Event
     */
    public function onConnect();

 	/**
     * @return Event
     */
    public function onClose();

 	/**
     * @return Event
     */
    public function onReceive();

 	/**
     * @return Event
     */
    public function onStop();
}