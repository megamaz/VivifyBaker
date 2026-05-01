using System;
using JetBrains.Annotations;

namespace VivifyBaker.Baker.Scripts.Editor.Utility
{
    public struct Point<T>
    {
        public T _values;
        public float _time;
        [CanBeNull] public string _easing;

        public string ToString()
        {
            return _values.ToString();
        }
    }
    public struct PointDefinition<T>
    {
        public Point<T>[] Points;
    }
}