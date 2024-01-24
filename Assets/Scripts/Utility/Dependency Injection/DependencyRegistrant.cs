using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DependencyRegistrant : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> dependencies;
    [SerializeField] private List<NamedDependency> namedDependencies;
    [SerializeField] private List<Transform> transformDependencies;

    // Must run before any scripts that use an injection attribute
    private void Awake()
    {
        foreach (var dependency in dependencies)
        {
            if (dependency != null)
            {
                InjectionContainer.Register(dependency.GetType(), dependency);
            }
            else
            {
                Debug.LogWarning($"Cannot register null dependency " +
                    $"in GameObject {gameObject.name}");
            }
        }

        foreach (var namedDependency in namedDependencies)
        {
            if (namedDependency.Dependency != null)
            {
                InjectionContainer.Register(
                    namedDependency.Name, namedDependency.Dependency);
            }
            else
            {
                Debug.LogWarning($"Cannot register null named dependency " +
                    $"in GameObject {gameObject.name}");
            }
        }

        foreach (var transform in transformDependencies)
        {
            if (transform != null)
            {
                InjectionContainer.Register(
                    $"{transform.gameObject.name}{nameof(Transform)}", transform);
            }
            else
            {
                Debug.LogWarning($"Cannot register null Transform dependency " +
                    $"in GameObject {gameObject.name}");
            }
        }
    }

    private void OnDestroy()
    {
        InjectionContainer.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Sort Dependencies")]
    private void SortDependencies()
    {
        Undo.RecordObject(this, "Sort Dependencies");

        List<string> duplicateDependencyNames = dependencies.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key.GetType().Name).ToList();

        if (duplicateDependencyNames.Count > 0)
        {
            Debug.LogWarning("Duplicate dependencies: " +
                $"{string.Join(", ", duplicateDependencyNames)}");
        }

        dependencies = dependencies.OrderBy(x => x.GetType().Name).ToList();
    }
#endif

    [Serializable]
    private class NamedDependency
    {
        public string Name;
        public MonoBehaviour Dependency;
    }
}
