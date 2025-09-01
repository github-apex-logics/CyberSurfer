using System;

namespace LightDI.Utils
{
    public static class TypeUtility
    {
        public static Type GetTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == name)
                        return type;
                }
            }

            return null;
        }
    }
}
