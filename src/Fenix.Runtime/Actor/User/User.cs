using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix 
{
    //User actor has a client
    //User actor can communicate with both server/client

    public partial class User : Actor
    {
        public User(string name) : base(name)
        {
        }
    }
}
