<?php

namespace bookrpg\util;

/**
* 
*/
class ByteArray
{
	private $data;
	private $position;
	private $length;

	public function readBoolean()
    {
		return json_encode($data,  JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
	}

	public function readByte()
	{
		return json_decode($data, true);
	}

	public function readInt16()
	{
		return json_decode($data, true);
	}

	public function readUInt16()
	{
		return json_decode($data, true);
	}
	public function readInt32()
	{
		return json_decode($data, true);
	}

	public function readUInt32()
	{
		return json_decode($data, true);
	}

	public function readInt64()
	{
		return json_decode($data, true);
	}

	public function readUInt64()
	{
		return json_decode($data, true);
	}

	public function readFloat()
	{
		return json_decode($data, true);
	}

	public function readDouble()
	{
		return json_decode($data, true);
	}

	public function readBytes()
	{
		return json_decode($data, true);
	}

	public function readString()
	{
		return json_decode($data, true);
	}


}

