using Fenix;
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
        delegate void cb(MatchCode code);

        public Avatar(): base()
        {
            //GetService("MatchService").add_to_match();

            Global.Get("MatchService").add_to_match(this.Get<User>().uid, 0, new Action<MatchCode>((code)=> { 
                
            }));
        }
    }
}
