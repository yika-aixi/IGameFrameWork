//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//2018年09月03日-09:31
//Icarus.UnityGameFramework.Runtime

using System;
using Icarus.GameFramework;
using Icarus.GameFramework.Event;

namespace Icarus.UnityGameFramework.Runtime
{
    public static class EventComponentExpansion
    {
        /// <summary>
        /// GameEventArgs 事件注册
        /// </summary>
        /// <param name="self"></param>
        /// <param name="handle"></param>
        /// <typeparam name="T"></typeparam>
        public static void GameEventSubscribe<T>(this EventComponent self,EventHandler<GameEventArgs> handle) where T : GameEventArgs, new()
        {
            var args =  ReferencePool.Acquire<T>();
            
            self.Subscribe(args.Id,handle);
            
            ReferencePool.Release(args);
        }
        
        /// <summary>
        /// GameEventArgs 事件注册
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameEventArgsType"></param>
        /// <param name="handle"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public static void GameEventSubscribe(this EventComponent self,Type gameEventArgsType,EventHandler<GameEventArgs> handle)
        {
            if (!typeof(GameEventArgs).IsAssignableFrom(gameEventArgsType))
            {
                throw new GameFrameworkException($"gameEventArgsType 不是 {typeof(GameEventArgs)} 类型");
            }
            
            var args =  (GameEventArgs)ReferencePool.Acquire(gameEventArgsType);
            
            self.Subscribe(args.Id,handle);
            
            ReferencePool.Release(args);
        }
        
        /// <summary>
        /// GameEventArgs 事件释放
        /// </summary>
        /// <param name="self"></param>
        /// <param name="handle"></param>
        /// <typeparam name="T"></typeparam>
        public static void GameEventUnsubscribe<T>(this EventComponent self,EventHandler<GameEventArgs> handle) where T : GameEventArgs, new()
        {
            var args =  ReferencePool.Acquire<T>();
            
            self.Unsubscribe(args.Id,handle);
            
            ReferencePool.Release(args);
        }
        
        /// <summary>
        /// GameEventArgs 事件释放
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameEventArgsType"></param>
        /// <param name="handle"></param>
        /// <exception cref="GameFrameworkException"></exception>
        public static void GameEventUnsubscribe(this EventComponent self,Type gameEventArgsType,EventHandler<GameEventArgs> handle)
        {
            if (!typeof(GameEventArgs).IsAssignableFrom(gameEventArgsType))
            {
                throw new GameFrameworkException($"gameEventArgsType 不是 {typeof(GameEventArgs)} 类型");
            }
            
            var args =  (GameEventArgs)ReferencePool.Acquire(gameEventArgsType);
            
            self.Unsubscribe(args.Id,handle);
            
            ReferencePool.Release(args);
        }
    }
}