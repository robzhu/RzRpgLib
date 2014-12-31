using RzAspects;

namespace RzRpgLib
{
    public interface IValueSource : ICompositeProperty
    {
        float Value { get; }
    }

    public class ValueSource : CompositePropertyChangeNotificationBase, IValueSource
    {
        public string PropertyValue { get { return "Value"; } }
        private float _Value;
        public virtual float Value
        {
            get { return _Value; }
            protected set { SetProperty( PropertyValue, ref _Value, value ); }
        }
    }
}
