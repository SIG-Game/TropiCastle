using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void InjectDependencies(this MonoBehaviour monoBehaviour)
    {
        IEnumerable<FieldInfo> injectFields = monoBehaviour.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(field => Attribute.IsDefined(field, typeof(InjectAttribute)));

        foreach (var injectField in injectFields)
        {
            if (InjectionContainer.TryGetDependency(
                injectField.FieldType, out MonoBehaviour dependency))
            {
                injectField.SetValue(monoBehaviour, dependency);
            }
            else if (!Attribute.IsDefined(injectField, typeof(OptionalInjectAttribute)))
            {
                Debug.LogError($"Unable to inject dependency of type {injectField.FieldType} " +
                    $"in GameObject {monoBehaviour.gameObject.name}.");
            }
        }
    }
}
