using System;

namespace RzRpgLib
{
    public enum NetStatisticModifierEffect
    {
        None,
        Neutral,    //Net Modifiers have 0 effect.  
        Positive,   //Net Modifiers are increasing the value above the base level
        Negative,   //Net Modifiers are decreasing the value below the base level
    }

    public interface IStatistic : IValueSource
    {
        bool HigherIsBetter { get; set; }
        bool IsPercentage { get; set; }
        float BaseValue { get; set; }
        float MinValue { get; set; }
        float MaxValue { get; set; }
        float AddTotal { get; set; }
        float MultiplyTotal { get; set; }

        NetStatisticModifierEffect NetEffect { get; }
    }

    public class Statistic : ValueSource, IStatistic
    {
        public string PropertyHigherIsBetter { get { return "HigherIsBetter"; } }
        private bool _HigherIsBetter = true;
        public bool HigherIsBetter
        {
            get { return _HigherIsBetter; }
            set { SetProperty( PropertyHigherIsBetter, ref _HigherIsBetter, value ); }
        }

        public string PropertyIsPercentage { get { return "IsPercentage"; } }
        private bool _IsPercentage = false;
        public bool IsPercentage
        {
            get { return _IsPercentage; }
            set { SetProperty( PropertyIsPercentage, ref _IsPercentage, value ); }
        }

        public string PropertyBaseValue { get { return "BaseValue"; } }
        private float _BaseValue;
        public float BaseValue
        {
            get { return _BaseValue; }
            set { SetProperty( PropertyBaseValue, ref _BaseValue, value, Refresh ); }
        }

        public string PropertyMinValue { get { return "MinValue"; } }
        private float _MinValue;
        public float MinValue
        {
            get { return _MinValue; }
            set { SetProperty( PropertyMinValue, ref _MinValue, value, Refresh ); }
        }

        public string PropertyMaxValue { get { return "MaxValue"; } }
        private float _MaxValue;
        public float MaxValue
        {
            get { return _MaxValue; }
            set { SetProperty( PropertyMaxValue, ref _MaxValue, value, Refresh ); }
        }

        public string PropertyAddTotal { get { return "AddTotal"; } }
        private float _AddTotal;
        public float AddTotal
        {
            get { return _AddTotal; }
            set { SetProperty( PropertyAddTotal, ref _AddTotal, value, Refresh ); }
        }

        public string PropertyMultiplyTotal { get { return "MultiplyTotal"; } }
        private float _MultiplyTotal = 1.0f;
        public float MultiplyTotal
        {
            get { return _MultiplyTotal; }
            set {
                value = (float)Math.Round( (double)value, 4, MidpointRounding.AwayFromZero );
                SetProperty( PropertyMultiplyTotal, ref _MultiplyTotal, value, Refresh ); 
            }
        }

        public string PropertyNetEffect { get { return "NetEffect"; } }
        private NetStatisticModifierEffect _NetEffect = NetStatisticModifierEffect.None;
        public NetStatisticModifierEffect NetEffect
        {
            get { return _NetEffect; }
            private set { SetProperty( PropertyNetEffect, ref _NetEffect, value ); }
        }

        public Statistic( float baseValue = 0, float minValue = float.MinValue, float maxValue = float.MaxValue )
        {
            BaseValue = baseValue;
            MinValue = minValue;
            MaxValue = maxValue;
            Refresh();
        }

        private void Refresh()
        {
            if( MinValue > MaxValue )
            {
                throw new ArgumentException( "violated: minValue < MaxValue" );
            }

            bool hasModifiers = ( AddTotal != 0 ) || ( MultiplyTotal != 1.0f );
            Value = ( BaseValue + AddTotal ) * MultiplyTotal;

            if( Value > BaseValue )
                NetEffect = NetStatisticModifierEffect.Positive;
            else if( Value < BaseValue )
                NetEffect = NetStatisticModifierEffect.Negative;
            else if( ( Value == BaseValue ) && hasModifiers )
                NetEffect = NetStatisticModifierEffect.Neutral;
            else
                NetEffect = NetStatisticModifierEffect.None;

            if( Value < MinValue ) Value = MinValue;
            if( Value > MaxValue ) Value = MaxValue;
        }
    }
}
