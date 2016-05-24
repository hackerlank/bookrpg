namespace bookrpg.resource
{
    public interface ICodec
    {
        string name{ get; }

        byte[] Encode(byte[] bytes);

        byte[] Decode(byte[] bytes);
    }
}
