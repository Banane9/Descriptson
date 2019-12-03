using System;
using System.Linq;
using System.Linq.Expressions;

namespace Descriptson
{
    public class CalcOperation
    {
        private readonly Func<Expression, Expression, Expression> getExpression;
        public string Name { get; private set; }
        public bool SingleOperand { get; private set; }

        public CalcOperation(string name, Func<Expression, Expression, Expression> getExpression, bool singleOperand = false)
        {
            Name = name;
            this.getExpression = getExpression;
            SingleOperand = singleOperand;
        }

        public Expression GetExpression(Expression firstOperand, Expression secondOperand)
        {
            if (SingleOperand && secondOperand != null)
                throw new ArgumentException("Single operand operation shouldn't have a second operand set!", nameof(secondOperand));

            return getExpression(firstOperand, secondOperand);
        }

        public Expression GetExpression(params Expression[] operands)
        {
            if (SingleOperand && operands.Length > 1)
                throw new ArgumentException("Single operand operation shouldn't have more than one operand!", nameof(operands));

            if (SingleOperand)
                return getExpression(operands[0], null);

            Expression curExpression = operands[0];

            foreach (var operand in operands.Skip(1))
                curExpression = getExpression(curExpression, operand);

            return curExpression;
        }
    }
}