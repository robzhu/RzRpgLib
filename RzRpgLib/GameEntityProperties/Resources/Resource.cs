using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// A resource expresses something that can be consumed, regenerated, and potentially placed on cooldown.
    /// </summary>
    /// <example>HP, Mana, experience, rage, energy, global cooldown, ammo, heat, gold</example>
    public interface IResource : IValueSource
    {
        /// <summary>
        /// Gets or sets whether the resource is available for use.
        /// </summary>
        bool IsAvailable { get; set; }

        /// <summary>
        /// The cooldown for using this resource.  Defaults to 0.
        /// </summary>
        ICooldown Cooldown { get; }

        /// <summary>
        /// Depletes a specified amount of this resource and places it on cooldown.
        /// </summary>
        /// <param name="amount">The amount of this resource to use.</param>
        bool Use( float amount = 0 );

        /// <summary>
        /// The minimum value for this resource.
        /// </summary>
        IStatistic MinValue { get; }

        /// <summary>
        /// The maximum value for this resource.
        /// </summary>
        IStatistic MaxValue { get; }

        float CurrentValue { get; set; }

        bool IsMaxValue { get; }
        bool IsMinValue { get; }

        void SetMaxValue();
    }

    public class Resource : ValueSource, IResource
    {
        public string PropertyCurrentValue { get { return "CurrentValue"; } }
        private float _CurrentValue;
        public float CurrentValue
        {
            get { return _CurrentValue; }
            set 
            {
                if( value >= MaxValue.Value ) value = MaxValue.Value;
                else if( value <= MinValue.Value ) value = MinValue.Value;
                if( SetProperty( PropertyCurrentValue, ref _CurrentValue, value ) )
                {
                    Value = CurrentValue;
                }

                IsMaxValue = ( _CurrentValue >= MaxValue.Value );
                IsMinValue = ( _CurrentValue <= MinValue.Value );
                RefreshIsAvailable();
            }
        }

        public string PropertyIsMaxValue { get { return "IsMaxValue"; } }
        private bool _IsMaxValue = false;
        public bool IsMaxValue
        {
            get { return _IsMaxValue; }
            private set { SetProperty( PropertyIsMaxValue, ref _IsMaxValue, value ); }
        }

        public string PropertyIsMinValue { get { return "IsMinValue"; } }
        private bool _IsMinValue;
        public bool IsMinValue
        {
            get { return _IsMinValue; }
            private set { SetProperty( PropertyIsMinValue, ref _IsMinValue, value ); }
        }

        public string PropertyIsAvailable { get { return "IsAvailable"; } }
        private bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set { SetProperty( PropertyIsAvailable, ref _IsAvailable, value ); }
        }

        public string PropertyCooldown { get { return "Cooldown"; } }
        private ICooldown _Cooldown = new Cooldown();
        public ICooldown Cooldown { get { return _Cooldown; } }

        public string PropertyMaxValue { get { return "MaxValue"; } }
        private IStatistic _MaxValue = new Statistic();
        public IStatistic MaxValue { get { return _MaxValue; } }

        public string PropertyMinValue { get { return "MinValue"; } }
        private IStatistic _MinValue = new Statistic();
        public IStatistic MinValue { get { return _MinValue; } }

        private object _sync = new object();
        private float _lastMaxValue;

        public static Resource CreateResourceWithCooldown( float value, float cooldown )
        {
            var resource = new Resource( value );
            resource.Cooldown.Period.BaseValue = cooldown;
            return resource;
        }

        public Resource( float value = 0 ) : this( value, value ) { }
        public Resource( float value, float maxValueBase, float minValueBase = 0, float cooldown = 0 )
        {
            MinValue.PropertyChanged += ( s, e ) => CheckCurrentValueLimits();

            MaxValue.PropertyChanging += ( s, e ) => { _lastMaxValue = MaxValue.Value; };
            MaxValue.PropertyChanged += ( s, e ) =>
                {
                    float maxValueDelta = ( MaxValue.Value - _lastMaxValue );
                    if( maxValueDelta > 0 )
                    {
                        CurrentValue += maxValueDelta;
                    }
                    CheckCurrentValueLimits();
                };

            MaxValue.BaseValue = maxValueBase;
            MinValue.BaseValue = minValueBase;
            CurrentValue = value;
            Cooldown.Period.BaseValue = cooldown;
            Cooldown.IsAvailableChanged += ( cd ) =>
                {
                    IsAvailable = Cooldown.IsAvailable;
                };
        }

        private void CheckCurrentValueLimits()
        {
            if( Value >= MaxValue.Value ) CurrentValue = MaxValue.Value;
            if( Value <= MinValue.Value ) CurrentValue = MinValue.Value;
        }

        public bool Use(float amount = 0)
        {
            lock (_sync)
            {
                if (IsAvailable && (Value >= amount))
                {
                    CurrentValue -= amount;
                    Cooldown.Use();

                    RefreshIsAvailable();
                    return true;
                }
                return false;
            }
        }

        private void RefreshIsAvailable()
        {
            IsAvailable = Cooldown.IsAvailable;
        }

        public void SetMaxValue()
        {
            CurrentValue = MaxValue.Value;
        }
    }
}
