using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LightDI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
    };

    public interface IInjectable
    {
        void PostInject();
    }

    public sealed class InjectionManager : MonoBehaviour
    {
        private static readonly List<ISystem> _systemsContainer = new();
        private static readonly Dictionary<Type, IInjectable> _objectsContainer = new();

        public static bool ContainsSystem(Type systemType)
        {
            return _systemsContainer.FindIndex(systemType.IsInstanceOfType) >= 0;
        }

        public static void RegisterSystem<T>(T systemInstance) where T : MonoBehaviour, ISystem
        {
            if (!_systemsContainer.Contains(systemInstance))
            {
                _systemsContainer.Add(systemInstance);

                foreach (var sys in _systemsContainer)
                {
                    InjectTo(sys);
                }
            }
        }

        public static void UnregisterSystem<T>(T systemInstance) where T : MonoBehaviour, ISystem
        {
            if (_systemsContainer.Contains(systemInstance))
            {
                _systemsContainer.Remove(systemInstance);
            }
        }

        public static T CreateGameObject<T>() where T : MonoBehaviour, IInjectable
        {
            var go = new GameObject();
            var obj = go.AddComponent<T>();

            InjectTo(obj);
            obj.PostInject();
            RegisterObject(obj);

            return obj;
        }

        public static T CreateObject<T>() where T : IInjectable
        {
            T obj = default;

            InjectTo(obj);
            obj?.PostInject();
            RegisterObject(obj);

            return obj;
        }

        public static void RegisterObject<T>(T obj) where T : IInjectable
        {
            if (obj == null)
            {
                obj = default;
                InjectTo(obj);
            }

            var type = typeof(T);

            _objectsContainer[type] = obj;

            foreach (var sys in _systemsContainer)
            {
                InjectTo(sys);
            }

            foreach (var o in _objectsContainer)
            {
                InjectTo(o.Value);
            }

            obj?.PostInject();
        }

        private static void InjectTo<T>(T instance)
        {
            InjectTo(instance, typeof(T));
            InjectTo(instance, instance.GetType());
        }

        private static void InjectTo<T>(T instance, Type monoType)
        {
            var objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var t in objectFields)
            {
                if (Attribute.GetCustomAttribute(t, typeof(InjectAttribute)) is InjectAttribute attribute)
                {
                    var injectType = t.FieldType;
                    var system = _systemsContainer.Find(x => injectType.IsInstanceOfType(x));

                    if (system != null)
                    {
                        t.SetValue(instance, system);
                    }
                    else if (_objectsContainer.TryGetValue(injectType, out var obj))
                    {
                        t.SetValue(instance, obj);
                    }
                }
            }
        }


        public static void ResetInjection()
        {
            _systemsContainer.Clear();
            _objectsContainer.Clear();
        }


        public static InjectionManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton guard: prevent duplicates
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystems();
        }
        private static void InitializeSystems()
        {
            _systemsContainer.AddRange(FindObjectsOfType<MonoBehaviour>(true).OfType<ISystem>());

            foreach (var sys in _systemsContainer)
            {
                InjectTo(sys);
                var go = (sys as MonoBehaviour)?.gameObject;
               // DontDestroyOnLoad(go);
            }
        }
    }
}
