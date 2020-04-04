using System;

namespace Geburtstag
{
    public static class Validation
    {
        private static double LimitLog = Math.Log(long.MaxValue);
        private static double LimitLogLog = Math.Log(Math.Log(long.MaxValue));

        public static bool ValidSum(long A, long B)
        {
            return long.MaxValue - A - B > 0;
        }

        public static bool ValidDifference(long A, long B)
        {
            return A > B;
        }

        public static bool ValidProduct(long A, long B)
        {
            return LimitLog >= Math.Log(A) + Math.Log(B);
        }

        public static bool ValidQuotient(long A, long B)
        {
            return A % B == 0;
        }

        public static bool ValidPower(long A, long B)
        {
            return LimitLogLog >= Math.Log(A) + Math.Log(Math.Log(B));
        }

        public static bool ValidFactorial(long A)
        {
            return A <= 20;
        }
    }

    public abstract class Expression
    {
        protected string OperationString;
        protected Expression A;
        protected Expression B;
        public long Value { get; protected set; }

        public Expression(Expression A, Expression B)
        {
            this.A = A;
            this.B = B;
        }

        public override string ToString()
        {
            return "(" + A.ToString() + OperationString + B.ToString() + ")";
        }
    }

    public class ConstantExpression : Expression
    {
        public ConstantExpression(long _Value) : base(null, null)
        {
            Value = _Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class AddExpression : Expression
    {
        public AddExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "+";
            Value = A.Value + B.Value;
        }
    }

    public class SubtractExpression : Expression
    {
        public SubtractExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "-";
            Value = A.Value - B.Value;
        }
    }

    public class MultiplyExpression : Expression
    {
        public MultiplyExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "*";
            Value = A.Value * B.Value;
        }
    }

    public class DivideExpression : Expression
    {
        public DivideExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "/";
            Value = A.Value / B.Value;
        }
    }

    public class PowerExpression : Expression
    {
        public PowerExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "^";
            Value = (long)Math.Pow(A.Value, B.Value);
        }
    }

    public class ModuloExpression : Expression
    {
        public ModuloExpression(Expression A, Expression B) : base(A, B)
        {
            OperationString = "%";
            Value = A.Value % B.Value;
        }
    }

    public class FactorialExpression : Expression
    {
        public FactorialExpression(Expression A) : base(A, null)
        {
            OperationString = "!";
            Value = A.Value;
            for (long I = A.Value - 1; I >= 1; I--)
            {
                Value = Value * I;
            }
        }

        public override string ToString()
        {
            return "(" + A.ToString() + OperationString + ")";
        }
    }
}