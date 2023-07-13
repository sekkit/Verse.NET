using System.Collections.Generic;
using MemoryPack;

namespace DataModel.Shared.Model
{
    public partial class Model : Dictionary<string, object>
    {
        public T Get<T>(string name) where T: class
        {
            return this[name] as T;
        }
    }
}