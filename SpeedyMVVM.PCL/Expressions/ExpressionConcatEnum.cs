using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Expressions
{
    /// <summary>
    /// Concat Expression Enumerator.
    /// </summary>
    public enum ExpressionConcatEnum
    {
        /// <summary>
        /// AND operation bit by bit.
        /// </summary>
        And,
        /// <summary>
        /// OR opertion bit by bit.
        /// </summary>
        Or,
        /// <summary>
        /// AND operation bit by bit (Negated)
        /// </summary>
        AndNot,
        /// <summary>
        /// OR operation bit by bit (Negated)
        /// </summary>
        OrNot
    }
}
