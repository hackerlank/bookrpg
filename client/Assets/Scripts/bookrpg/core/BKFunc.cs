using System;

namespace bookrpg.core
{
    public delegate TReturn BKFunc<TReturn>();
    public delegate TReturn BKFunc<T, TReturn>(T arg);
    public delegate TReturn BKFunc<T1,T2,TReturn>(T1 arg1,T2 arg2);
    public delegate TReturn BKFunc<T1,T2,T3,TReturn>(T1 arg1,T2 arg2,T3 arg3);
    public delegate TReturn BKFunc<T1,T2,T3,T4,TReturn>(T1 arg1,T2 arg2,T3 arg3,T4 arg4);
    public delegate TReturn BKFunc<T1,T2,T3,T4,T5,TReturn>(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5);
    public delegate TReturn BKFunc<T1,T2,T3,T4,T5,T6,TReturn>(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6);
}
