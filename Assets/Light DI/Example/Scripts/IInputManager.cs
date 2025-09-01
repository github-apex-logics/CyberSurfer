using UnityEngine;

namespace LightDI.Examples
{
    public interface IInputManager : ISystem
    {
        Vector3 GetMovementDirection();
    }
}