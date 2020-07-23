using DotNetty.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fenix
{
    public class DbManager
    {
        protected DbManager()
        {

        }

        public static DbManager Instance = new DbManager();

        public string CreateUid()
        {
            return "1";
        }
    }
}
