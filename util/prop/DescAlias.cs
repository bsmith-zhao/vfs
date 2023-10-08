using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace util.prop
{
    public class DescAlias : Attribute
    {
        Type owner;

        public DescAlias(Type owner)
            => this.owner = owner;

        public Type aliasOwner() => owner;
    }
}
