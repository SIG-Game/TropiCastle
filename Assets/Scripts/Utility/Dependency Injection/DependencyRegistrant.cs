using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DependencyRegistrant : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> dependencies;

    // Must run before any scripts that use the Inject attribute
    private void Awake()
    {
        foreach (var dependency in dependencies)
        {
            InjectionContainer.Register(dependency.GetType(), dependency);
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

        dependencies = dependencies.OrderBy(x => x.GetType().Name).ToList();
    }
#endif
}
