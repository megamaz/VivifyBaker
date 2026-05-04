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
        
        public new string ToString()
        {
            return _values.ToString();
        }

        public static bool operator ==(Point<T> a, Point<T> b)
        {
            return a.Equals(b);
        }
        
        public static bool operator !=(Point<T> a, Point<T> b)
        {
            return !(a==b);
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Point<T> other)
            {
                return _time == other._time && _values.Equals(other._values);  
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public struct PointDefinition<T>
    {
        public Point<T>[] Points;
    }
}