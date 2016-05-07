using System.Collections;
using System.Collections.Generic;

namespace bookrpg.core
{
    public class CountableRef
    {
        public int count = 0;
        public bool cache = false;
        public object target = null;

        public CountableRef(object target = null, bool cache = false)
        {
            this.target = target;
            this.cache = cache;
        }

        public object refTarget()
        {
            count++;
            return target;
        }

        public void deRefTarget()
        {
            count--;
        }

        public bool canDisposed()
        {
            return !cache && count <= 0;
        }
    }
}
