using System;
using RzAspects;

namespace RzRpgLib
{
    //Deals [50] damage.

    //Describes the effect description
    public class EffectDescriptionPart : ModelBase
    {
        public string PropertyText { get { return "Text"; } }
        private string _Text;
        public string Text
        {
            get{ return _Text; }
            set{ SetProperty( PropertyText, ref _Text, value ); }
        }
    }

    /// <summary>
    /// This class modifies a target resource based on an equation that calculates the value using a variety of inputs.
    /// </summary>
    public class CalculatedResourceModifier : ResourceModifier
    {
        public ObservableCollectionEx<EffectDescriptionPart> DescriptionParts { get; private set; }
        public Func<float> ValueExpression { get; private set; }

        public CalculatedResourceModifier( string target, Func<float> expression ) : base( target, 0 )
        {
            DescriptionParts = new ObservableCollectionEx<EffectDescriptionPart>();
            ValueExpression = expression;

            DescendentPropertyChanged += (s, e) => RecalculateValue();
        }

        private void RecalculateValue()
        {
            Value = ValueExpression();
        }
    }
}
