using JetBrains.Annotations;

namespace VivifyBaker.Baker.Scripts.Editor.Utility
{
    public class Point<T>
    {
        private T _values;
        private float _time;
        [CanBeNull] private string _easing;
    }
    public class PointDefinition<T>
    {
        private Point<T>[] _points;
    }
}