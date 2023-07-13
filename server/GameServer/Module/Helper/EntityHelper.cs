using Module.Shared;
using Module.User;
using Module.User.Login;

namespace Module.Helper;

public static class EntityHelper
{
    public static Entity createEntityWithLogin()
    {
        var e = new Entity();
        e.AddModule<RpcModule>();
        e.AddModule<LoginModule>();
        return e;
    }
}