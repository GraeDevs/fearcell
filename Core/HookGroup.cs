using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fearcell.Core
{
    public class HookGroup : IOrderedLoadable
    {
        public virtual float Priority => 1f;

        public virtual void Load() { }

        public virtual void Unload() { }
    }
}
