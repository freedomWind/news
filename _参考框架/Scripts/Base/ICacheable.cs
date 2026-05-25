using GameFramework;
using System;

public interface ICacheable : GameFramework.IReference
{}
    
public static class ObjectCache
{
    public static void Cache(ICacheable item)
    {
        ReferencePool.Release(item);            
    }

    public static void Cache<T>(T item) where T : ICacheable
    {
        ReferencePool.Release(item);
    }

    public static T Acquire<T>() where T : class, ICacheable,new()
    {
        return ReferencePool.Acquire<T>();
    }

    public static ICacheable Acquire(Type cacheType)
    {
        return ReferencePool.Acquire(cacheType) as ICacheable;
    }
}