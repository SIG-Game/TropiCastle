using System;
using System.Collections.Generic;
using UnityEngine;

public static class InjectionContainer
{
    private readonly static Dictionary<Type, MonoBehaviour> dependencies;
    private readonly static Dictionary<string, Component> namedDependencies;

    static InjectionContainer()
    {
        dependencies = new Dictionary<Type, MonoBehaviour>();
        namedDependencies = new Dictionary<string, Component>();
    }

    public static void Register(Type dependencyType, MonoBehaviour dependency)
    {
        dependencies[dependencyType] = dependency;
    }

    public static void Register(string dependencyName, Component dependency)
    {
        namedDependencies[dependencyName] = dependency;
    }

    public static void Clear()
    {
        dependencies.Clear();
        namedDependencies.Clear();
    }

    public static bool TryGetDependency(Type dependencyType, out MonoBehaviour dependency) =>
        dependencies.TryGetValue(dependencyType, out dependency);

    public static bool TryGetDependency(string dependencyName, out Component dependency) =>
        namedDependencies.TryGetValue(dependencyName, out dependency);
}
