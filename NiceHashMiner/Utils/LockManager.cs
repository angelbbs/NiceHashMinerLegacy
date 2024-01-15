using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Utils
{
    class lobj
    {
        public int count = 0;
    }

    public class LockManager
    {
        static ConcurrentDictionary<string, lobj> _locks =
                new ConcurrentDictionary<string, lobj>();
        private static lobj GetLock(string filename)
        {
            lobj o = null;
            if (_locks.TryGetValue(filename.ToLower(), out o))
            {
                o.count++;
                return o;
            }
            else
            {
                o = new lobj();
                _locks.TryAdd(filename.ToLower(), o);
                o.count++;
                return o;
            }
        }

        public static void GetLock(string filename, Action action)
        {
            lock (GetLock(filename))
            {
                action();
                Unlock(filename);
            }
        }

        private static void Unlock(string filename)
        {
            lobj o = null;
            if (_locks.TryGetValue(filename.ToLower(), out o))
            {
                o.count--;
                if (o.count == 0)
                    _locks.TryRemove(filename.ToLower(), out o);
            }
        }
    }
}
