using System;
using JetBrains.Annotations;

namespace VivifyBaker.Baker.Scripts.Editor.Utility
{
    public class Point<T>
    {
        public T _values;
        public float _time;
        [CanBeNull] public string _easing;
        
        public Point(float time, T value)
        {
            _time = time;
            _values = value;
        }

        public Point<T> Copy()
        {
            return new Point<T>(this._time, this._values);
        }
        
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