<?php
namespace bookrpg\socket;

interface IClient
{
	/**
	 * seconds
	 */
	public function getTimeout();

	/**
	 * seconds
	 */
	public function setTimeout($value);

	public function clientIP();

	public function clientPort();

	public function remoteIP();

	public function remotePort();

	/**
     * @return bool
     */
	public function connected();

	public function connect($host, $port);

	public function send($message);
	
	public function close();
}