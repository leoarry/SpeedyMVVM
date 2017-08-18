
namespace SpeedyMVVM.Expressions
{
    /// <summary>
    /// Define an operator for an expression.
    /// </summary>
    public enum ExpressionOperatorEnum
    {
        /// <summary>
        /// Compare 'Member' equals as 'Constant'.
        /// </summary>
        Equals,
        /// <summary>
        /// Compare 'Member' NOT equal as 'Constant'.
        /// </summary>
        Different,
        /// <summary>
        /// Compare 'Member' greater than 'Constant'.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Compare 'Member' less than 'Constant'.
        /// </summary>
        LessThan,
        /// <summary>
        /// Compare 'Member' greater than or equal as 'Constant'.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// Compare 'Member' less than or equal as 'Constant'.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 'Member' contains 'Constant' (string).
        /// </summary>
        Contains,
        /// <summary>
        /// 'Member' start with 'Constant' (string).
        /// </summary>
        StartsWith,
        /// <summary>
        /// 'Member' end with 'Constant' (string).
        /// </summary>
        EndsWith,
        /// <summary>
        /// 'Member' is null or empty (string).
        /// </summary>
        IsNullOrEmpty
    }
    }
