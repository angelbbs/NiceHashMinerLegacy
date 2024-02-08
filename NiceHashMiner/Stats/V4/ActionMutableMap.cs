using System.Collections.Generic;
using System.Linq;

namespace NiceHashMiner.Stats.V4
{
    public static class ActionMutableMap
    {
        private static List<NhmwsAction> _actionList = new List<NhmwsAction>();
        private static List<OptionalMutableProperty> _mutableList = new List<OptionalMutableProperty>();
        private static readonly object _lock = new object();

        public static List<NhmwsAction> ActionList
        {
            get
            {
                lock(_lock )
                {
                    return _actionList;
                }
            }
            set
            {
                lock(_lock )
                {
                    _actionList = value;
                }
            }
        }
        public static List<OptionalMutableProperty> MutableList
        {
            get
            {
                lock(_lock )
                {
                    return _mutableList;
                }
            }
            set
            {
                lock(_lock )
                {
                    _mutableList = value;
                }
            }
        }

        public static NhmwsAction FindActionOrNull(int actionID)
        {
            var actionRecord = ActionList.Where(a => a.ActionID == actionID).FirstOrDefault();
            if (actionRecord != null) return actionRecord;
            return null;
        }
        public static OptionalMutableProperty FindMutableOrNull(int propID)
        {
            var mutableRecord = MutableList.Where(a => a.PropertyID == propID).FirstOrDefault();
            if (mutableRecord != null) return mutableRecord;
            return null;
        }
        public static void ResetArrays()
        {
            ActionList.Clear();
            MutableList.Clear();
        }
    }
}
