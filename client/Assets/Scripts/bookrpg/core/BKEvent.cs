using System;
using System.Collections;
using System.Collections.Generic;

namespace bookrpg.core
{
    public class BKEvent : BKEventBase
    {
        private BKAction action;
        private BKAction onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction action)
        {
            this.action += action;
        }

        public void Invoke()
        {
            if (this.action != null)
            {
                this.action.Invoke();
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke();
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove()
        {
            if (this.action != null)
            {
                this.action.Invoke();
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke();
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction action)
        {
            this.action -= action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public static BKEvent operator +(BKEvent evt, BKAction action)
        {
            evt.action += action;
            return evt;
        }

        public static BKEvent operator -(BKEvent evt, BKAction action)
        {
            evt.action -= action;
            return evt;
        }
    }

    public class BKEvent<T> : BKEventBase
    {
        private BKAction<T> action;
        private BKAction<T> onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction<T> action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction<T> action)
        {
            this.action += action;
        }

        public void Invoke(T arg)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg);
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg);
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove(T arg)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg);
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg);
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction<T> action)
        {
            this.action -= action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public static BKEvent<T> operator +(BKEvent<T> evt, BKAction<T> action)
        {
            evt.action += action;
            return evt;
        }

        public static BKEvent<T> operator -(BKEvent<T> evt, BKAction<T> action)
        {
            evt.action -= action;
            return evt;
        }
    }

    public class BKEvent<T1, T2> : BKEventBase
    {
        private BKAction<T1, T2> action;
        private BKAction<T1, T2> onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction<T1, T2> action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction<T1, T2> action)
        {
            this.action += action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2);
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2);
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove(T1 arg1, T2 arg2)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2);
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2);
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction<T1, T2> action)
        {
            this.action -= action;
        }

        public static BKEvent<T1, T2> operator +(BKEvent<T1, T2> evt, BKAction<T1, T2> action)
        {
            evt.action += action;
            return evt;
        }


        public static BKEvent<T1, T2> operator -(BKEvent<T1, T2> evt, BKAction<T1, T2> action)
        {
            evt.action -= action;
            return evt;
        }
    }

    public class BKEvent<T1, T2, T3> : BKEventBase
    {
        private BKAction<T1, T2, T3> action;
        private BKAction<T1, T2, T3> onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction<T1, T2, T3> action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction<T1,T2,T3> action)
        {
            this.action += action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3);
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3);
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove(T1 arg1, T2 arg2, T3 arg3)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3);
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3);
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction<T1, T2, T3> action)
        {
            this.action -= action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public static BKEvent<T1, T2, T3> operator +(BKEvent<T1, T2, T3> evt, BKAction<T1, T2, T3> action)
        {
            evt.action += action;
            return evt;
        }

        public static BKEvent<T1, T2, T3> operator -(BKEvent<T1, T2, T3> evt, BKAction<T1, T2, T3> action)
        {
            evt.action -= action;
            return evt;
        }
    }

    public class BKEvent<T1, T2, T3, T4> : BKEventBase
    {
        private BKAction<T1, T2, T3, T4> action;
        private BKAction<T1, T2, T3, T4> onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction<T1, T2, T3, T4> action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction<T1,T2,T3,T4> action)
        {
            this.action += action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3, arg4);
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3, arg4);
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3, arg4);
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3, arg4);
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction<T1, T2, T3, T4> action)
        {
            this.action -= action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public static BKEvent<T1, T2, T3, T4> operator +(BKEvent<T1, T2, T3, T4> evt, BKAction<T1, T2, T3, T4> action)
        {
            evt.action += action;
            return evt;
        }

        public static BKEvent<T1, T2, T3, T4> operator -(BKEvent<T1, T2, T3, T4> evt, BKAction<T1, T2, T3, T4> action)
        {
            evt.action -= action;
            return evt;
        }
    }

    public class BKEvent<T1, T2, T3, T4, T5> : BKEventBase
    {
        private BKAction<T1, T2, T3, T4, T5> action;
        private BKAction<T1, T2, T3, T4, T5> onceAction;

        /// <summary>
        /// Add event Listener, or BKEvent += action
        /// </summary>
        public void AddListener(BKAction<T1, T2, T3, T4, T5> action)
        {
            this.action += action;
        }

        /// <summary>
        /// action will be removed after calling once
        /// </summary>
        public void AddOnceListener(BKAction<T1,T2,T3,T4,T5> action)
        {
            this.action += action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3, arg4, arg5);
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3, arg4, arg5);
                this.onceAction = null;
            }
        }

        public void InvokeAndRemove(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (this.action != null)
            {
                this.action.Invoke(arg1, arg2, arg3, arg4, arg5);
                this.action = null;
            }

            if (this.onceAction != null)
            {
                this.onceAction.Invoke(arg1, arg2, arg3, arg4, arg5);
                this.onceAction = null;
            }
        }

        /// <summary>
        /// remove event Listener, or BKEvent -= action
        /// </summary>
        public void RemoveListener(BKAction<T1, T2, T3, T4, T5> action)
        {
            this.action -= action;
            this.onceAction -= action;
        }

        public override void RemoveAllListeners()
        {
            this.action = null;
            this.onceAction = null;
        }

        public static BKEvent<T1, T2, T3, T4, T5> operator +(BKEvent<T1, T2, T3, T4, T5> evt, BKAction<T1, T2, T3, T4, T5> action)
        {
            evt.action += action;
            return evt;
        }

        public static BKEvent<T1, T2, T3, T4, T5> operator -(BKEvent<T1, T2, T3, T4, T5> evt, BKAction<T1, T2, T3, T4, T5> action)
        {
            evt.action -= action;
            return evt;
        }
    }
}
