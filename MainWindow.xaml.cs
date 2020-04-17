using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;

namespace Geburtstag
{
    public partial class MainWindow : Window
    {
        private enum EMode { TaskA, TaskB }

        EMode Mode;
        long TargetNumber;
        int Digit;
        bool SolutionFound;
        bool UseModulo;
        Stopwatch Timer;
        SortedDictionary<long, byte> Calculated;
        List<List<Expression>> Expressions;

        public MainWindow()
        {
            InitializeComponent();
            GroupBoxSolutionA.Visibility = Visibility.Collapsed;
            GroupBoxSolutionB.Visibility = Visibility.Collapsed;

            Timer = new Stopwatch();
            Calculated = new SortedDictionary<long, byte>();
            Expressions = new List<List<Expression>>();
        }

        private void Calculate()
        {
            // Initialize
            SolutionFound = false;
            Calculated.Clear();
            Expressions.Clear();
            Timer.Restart();

            // Edge cases
            if (TargetNumber == 0)
            {
                Solution(new SubtractExpression(new ConstantExpression(Digit), new ConstantExpression(Digit)), 1);
                return;
            }

            if (Digit == TargetNumber)
            {
                Solution(new ConstantExpression(TargetNumber), 1);
                return;
            }

            bool Same = true;
            foreach(char C in TargetNumber.ToString())
            {
                if(C.ToString() != Digit.ToString()) { Same = false; break; }
            }

            if(Same)
            {
                Solution(new ConstantExpression(TargetNumber), TargetNumber.ToString().Length);
                return;
            }

            // Calculation
            double Base = 0;
            int I = 0;
            while (!SolutionFound)
            {
                Expressions.Add(new List<Expression>());

                // Calculate 'Base' | 1, 11, 111, 1111
                Base = Math.Pow(10, I) * Digit + Base;
                Expressions[I].Add(new ConstantExpression((long)Base));

                if (I == 0) { I++; continue; }

                // Factorial
                if (Mode == EMode.TaskB)
                {
                    AddFactorialExpressions(I - 1);
                }

                // Count up only to half the list (prevent further duplicates)
                int Limit = (I + 1) / 2;
                for (int J = 0; J < Limit; J++)
                {
                    int K = I - J - 1;
                    foreach (Expression A in Expressions[J])
                    {
                        foreach (Expression B in Expressions[K])
                        {
                            CombineExpressions(I, A, B);
                        }
                    }
                }

                I++;
            }
        }

        private void ParseInputs()
        {
            if (!long.TryParse(TextBoxNumber.Text.Trim(), out TargetNumber))
            {
                MessageBox.Show("Dieser Wert ist ungültig.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBoxDigit.Text.Trim(), out Digit) | Digit == 0)
            {
                MessageBox.Show("Diese Ziffer ist ungültig.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UseModulo = CheckBoxModulo.IsChecked == true;
        }

        public void AddFactorialExpressions(int Index)
        {
            // Factorial
            for (int I = 0; I < Expressions[Index].Count; I++)
            {
                Expression E = Expressions[Index][I];
                if (E.Value <= 2) { continue; }
                if (Validation.ValidFactorial(E.Value))
                {
                    SaveExpression(Index, new FactorialExpression(E));
                }
            }
        }

        private void CombineExpressions(int Index, Expression A, Expression B)
        {
            // Addition
            if (Validation.ValidSum(A.Value, B.Value))
            {
                SaveExpression(Index, new AddExpression(A, B));
            }

            // Subtraction
            if (Validation.ValidDifference(A.Value, B.Value))
            {
                SaveExpression(Index, new SubtractExpression(A, B));
            }
            else
            {
                SaveExpression(Index, new SubtractExpression(B, A));
            }

            // Multiplication
            if (Validation.ValidProduct(A.Value, B.Value))
            {
                SaveExpression(Index, new MultiplyExpression(A, B));
            }
            
            // Division
            if(Validation.ValidQuotient(A.Value, B.Value))
            {
                SaveExpression(Index, new DivideExpression(A, B));
            }
            if (Validation.ValidQuotient(B.Value, A.Value))
            {
                SaveExpression(Index, new DivideExpression(B, A));
            }

            // Power
            if (Mode == EMode.TaskB && Validation.ValidPower(A.Value, B.Value))
            {
                SaveExpression(Index, new PowerExpression(A, B));
            }
            if (Mode == EMode.TaskB && Validation.ValidPower(B.Value, A.Value))
            {
                SaveExpression(Index, new PowerExpression(B, A));
            }

            // Modulo
            if (UseModulo)
            {
                SaveExpression(Index, new ModuloExpression(A, B));
                SaveExpression(Index, new ModuloExpression(B, A));
            }
        }

        private void SaveExpression(int Index, Expression A)
        {
            if (SolutionFound) { return; }
            if (A.Value <= 0) { return; }

            // Check for solution
            if (A.Value == TargetNumber)
            {
                Solution(A, Index + 1);
                return;
            }

            // Ignore duplicates
            if (!Calculated.ContainsKey(A.Value))
            {
                Calculated.Add(A.Value, 0);
                Expressions[Index].Add(A);
            }
        }

        private void Solution(Expression Solution, int DigitCount)
        {
            Timer.Stop();
            if(Mode == EMode.TaskA)
            {
                LabelSolutionA.Text = Solution.ToString();
                LabelDigitCountA.Content = DigitCount.ToString();
                LabelTimeElapsedA.Content = Timer.Elapsed.ToString(@"mm\:ss\.fff");
                GroupBoxSolutionA.Visibility = Visibility.Visible;
            }
            else if (Mode == EMode.TaskB)
            {
                LabelSolutionB.Text = Solution.ToString();
                LabelDigitCountB.Content = DigitCount.ToString();
                LabelTimeElapsedB.Content = Timer.Elapsed.ToString(@"mm\:ss\.fff");
                GroupBoxSolutionB.Visibility = Visibility.Visible;
            }

            SolutionFound = true;
        }

        private void ButtonBerechnenA_Click(object sender, RoutedEventArgs e)
        {
            Mode = EMode.TaskA;
            ParseInputs();
            Calculate();
        }

        private void ButtonBerechnenB_Click(object sender, RoutedEventArgs e)
        {
            Mode = EMode.TaskB;
            ParseInputs();
            Calculate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
