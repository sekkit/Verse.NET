using Fenix;
using Fenix.Common;
using GModule.Match;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UModule
{
    [RuntimeData(typeof(User))]
    public partial class Avatar : Actor
    {
        public Avatar(string uid): base(uid)
        {

        }

        public override void Update()
        {
            base.Update();

            var svc = GetService("MatchService");
            svc.rpc_join_match("", 1, new Action<uint>((code) => 
            {
                Log.Info(code.ToString());
            }));
        }
    }
}
