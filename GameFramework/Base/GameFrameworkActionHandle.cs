//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年07月20日-01:03
//Icarus.GameFramework

namespace Icarus.GameFramework
{
    public static class GameFrameworkActionHandle
    {
        public static void Handle(this GameFrameworkAction action)
        {
            action?.Invoke();
        }

        public static void Handle<T1>(
            this GameFrameworkAction<T1> action,
            T1 arg1)
        {
            action?.Invoke(arg1);
        }

        public static void Handle<T1, T2>(
            this GameFrameworkAction<T1, T2> action,
            T1 arg1, T2 arg2)
        {
            action?.Invoke(arg1, arg2);
        }

        public static void Handle<T1, T2, T3>(
            this GameFrameworkAction<T1, T2, T3> action,
            T1 arg1, T2 arg2, T3 arg3)
        {
            action?.Invoke(arg1, arg2, arg3);
        }

        public static void Handle<T1, T2, T3, T4>(
            this GameFrameworkAction<T1, T2, T3, T4> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            action?.Invoke(arg1, arg2, arg3, arg4);
        }

        public static void Handle<T1, T2, T3, T4, T5>(
            this GameFrameworkAction<T1, T2, T3, T4, T5> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void Handle<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void Handle<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16>(this GameFrameworkAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

    }
}