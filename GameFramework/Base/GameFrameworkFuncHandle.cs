//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-01:03
//Icarus.GameFramework

namespace Icarus.GameFramework
{
    public static class GameFrameworkFuncHandle
    {
        public static TResult Handle<TResult>(this GameFrameworkFunc<TResult> action)
        {
            return action == null ? default(TResult) : action.Invoke();
        }

        public static TResult Handle<T1, TResult>(
            this GameFrameworkFunc<T1, TResult> action,
            T1 arg1)
        {
            return action == null ? default(TResult) : action.Invoke(arg1);
        }

        public static TResult Handle<T1, T2, TResult>(
            this GameFrameworkFunc<T1, T2, TResult> action,
            T1 arg1, T2 arg2)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2);
        }

        public static TResult Handle<T1, T2, T3, TResult>(
            this GameFrameworkFunc<T1, T2, T3, TResult> action,
            T1 arg1, T2 arg2, T3 arg3)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3);
        }

        public static TResult Handle<T1, T2, T3, T4, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static TResult Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this GameFrameworkFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return action == null ? default(TResult) : action.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

    }
}