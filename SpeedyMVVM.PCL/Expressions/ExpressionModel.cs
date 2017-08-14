using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Expressions
{
    /// <summary>
    /// Provide a model for expressions to use with ExpressionBuilder.
    /// </summary>
    public class ExpressionModel: ObservableObject
    {
        #region Fields
        private string _PropertyName;
        private ExpressionOperatorEnum _Operator;
        private object _Value;
        private ExpressionConcatEnum _ConcatOperator;
        #endregion

        #region Property
        /// <summary>
        /// Name of the property to query.
        /// </summary>
        public string PropertyName
        {
            get { return _PropertyName; }
            set
            {
                if (value != _PropertyName)
                {
                    _PropertyName = value;
                    OnPropertyChanged(nameof(PropertyName));
                }
            }
        }

        /// <summary>
        /// Comparator for the query.
        /// </summary>
        public ExpressionOperatorEnum Operator
        {
            get { return _Operator; }
            set
            {
                if (value != _Operator)
                {
                    _Operator = value;
                    OnPropertyChanged(nameof(Operator));
                }
            }
        }

        /// <summary>
        /// Value used for the comparation.
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set
            {
                if (value != _Value)
                {
                    _Value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Logical value to concat the next expression.
        /// </summary>
        public ExpressionConcatEnum ConcatOperator
        {
            get { return _ConcatOperator; }
            set
            {
                if (value != _ConcatOperator)
                {
                    _ConcatOperator = value;
                    OnPropertyChanged(nameof(ConcatOperator));
                }
            }
        }
        #endregion
    }
}
