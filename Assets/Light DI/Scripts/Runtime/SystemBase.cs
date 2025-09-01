using UnityEngine;

namespace LightDI
{
    public class SystemBase : MonoBehaviour, ISystem
    {
        protected virtual void Start()
        {
            InjectionManager.RegisterSystem(this);
        }

        protected virtual void OnDestroy()
        {
            InjectionManager.UnregisterSystem(this);
        }
    }
}