namespace Module.Shared
{
    public enum ProtoCode : uint
    {
        INVALID=0x0,
        VOID=0x1,
        LOGIN=0x2,
        TEST_NTF=0x3,
        
        ON_TEST = 0x10001,
        ON_SYNC_FIELD=0x10002,
    }
}