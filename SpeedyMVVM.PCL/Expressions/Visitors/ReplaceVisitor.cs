using System.Linq.Expressions;

namespace SpeedyMVVM.Expressions.Visitors
{
    /// <summary>
    /// Provide a class to replace an ExpressionVisitor into an expression.
    /// </summary>
    internal class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;

        public ReplaceVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }

}
