namespace bookrpg.Editor
{
    public interface ICodec
    {
        string name{ get; }

        byte[] encode(byte[] bytes);

        byte[] decode(byte[] bytes);
    }
}
