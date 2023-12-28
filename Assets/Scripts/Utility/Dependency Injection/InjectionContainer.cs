using System;
using System.Collections.Generic;
using UnityEngine;

public static class InjectionContainer
{
    private readonly static Dictionary<Type, MonoBehaviour> dependencies;

    static InjectionContainer()
    {
        dependencies = new Dictionary<Type, MonoBehaviour>();
    }

    public static void Register(Type dependencyType, MonoBehaviour dependency)
    {
        dependencies[dependencyType] = dependency;
    }

    public static void Clear() => dependencies.Clear();

    public static bool TryGetDependency(Type dependencyType, out MonoBehaviour dependency) =>
        dependencies.TryGetValue(dependencyType, out dependency);
}
