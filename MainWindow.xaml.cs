using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace logcalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = EquationsTextBox.Text.Trim();
                var (operators, betweenOperators) = SplitIntoLists(text);
                string equation = "";
                for (int i = 0; i < operators.Count; i++)
                {
                    equation += $"{Solvelog(betweenOperators[i])}{operators[i]}";
                }
                equation += $"{Solvelog(betweenOperators[betweenOperators.Count - 1])}";

                result.Text = new DataTable().Compute(equation, "") + "";
            }catch (Exception ex) { MessageBox.Show(ex.ToString(),"You Were wrong, and this is why"); }
        }

        static (List<char>, List<string>) SplitIntoLists(string text)
        {
            List<char> operators = new List<char>();
            List<string> betweenOperators = new List<string>();
            string currentFragment = "";

            foreach (char character in text)
            {
                if (IsOperator(character))
                {
                    if (!string.IsNullOrEmpty(currentFragment))
                    {
                        betweenOperators.Add(currentFragment);
                        currentFragment = "";
                    }
                    operators.Add(character);
                }
                else
                {
                    currentFragment += character;
                }
            }

            // Add the last fragment to the list between operators
            if (!string.IsNullOrEmpty(currentFragment))
            {
                betweenOperators.Add(currentFragment);
            }

            return (operators, betweenOperators);
        }

        static bool IsOperator(char character)
        {
            char[] operators = { '+', '-', '*', '/' };
            return Array.IndexOf(operators, character) != -1;
        }

        private double Solvelog(string equation)
        {
            double baseLog, argument;
            if (equation.Contains("_"))
            {
                // Extract the base
                int baseIndex = equation.IndexOf("_") + 1;
                int argumentIndex = equation.IndexOf("(");
                string baseStr = equation.Substring(baseIndex, argumentIndex - baseIndex);
                baseLog = Convert.ToDouble(baseStr);

                // Extract the argument
                int argumentEndIndex = equation.IndexOf(")");
                string argumentStr = equation.Substring(argumentIndex + 1, argumentEndIndex - argumentIndex - 1);
                argument = Convert.ToDouble(argumentStr);
            }
            else
            {
                // Default base
                baseLog = 10;

                // Extract the argument
                int argumentIndex = equation.IndexOf("(");
                int argumentEndIndex = equation.IndexOf(")");
                string argumentStr = equation.Substring(argumentIndex + 1, argumentEndIndex - argumentIndex - 1);
                argument = Convert.ToDouble(argumentStr);
            }

            // Calculate the logarithm
            double result = Math.Log(argument, baseLog);

            return result;
        }

        private void CheckAriButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<(string, int)> sequence = ParseInput(ariProgressTextBox.Text);
                string tmp = "";
                foreach(var data in sequence)
                {
                    tmp += $"{data.Item1}={data.Item2}, ";  
                }
                tmp += $"\n{MonotonicityOfArray(sequence)}";
                ariResult.Text = tmp;
            }catch (Exception ex) { MessageBox.Show(ex.ToString(), "You Were wrong, and this is why"); }
        }

        static List<(string, int)> ParseInput(string input)
        {
            List<(string, int)> dataList = new List<(string, int)>();

            input = Regex.Replace(input, @"\s", "");
            string[] pairs = input.Split(',');

            foreach (string pair in pairs)
            {
                string[] parts = pair.Split('=');

                if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                {
                    dataList.Add((parts[0], value));
                }
                else
                {
                   MessageBox.Show($"Incorrect format: {pair}.", "You Were wrong, and this is why");
                }
            }

            return dataList;
        }

        static string MonotonicityOfArray(List<(string, int)> dataList)
        {
            dataList.Sort((x, y) => string.Compare(x.Item1, y.Item1));

            int prevValue = dataList[0].Item2;
            bool increasing = true;
            bool decreasing = true;
            bool constant = true;

            for (int i = 1; i < dataList.Count; i++)
            {
                if (dataList[i].Item2 != prevValue)
                {
                    constant = false;
                }
                if (dataList[i].Item2 > prevValue)
                {
                    decreasing = false;
                }
                else if (dataList[i].Item2 < prevValue)
                {
                    increasing = false;
                }

                prevValue = dataList[i].Item2;
            }

            if (constant)
            {
                return "Stały";
            }
            else if (decreasing)
            {
                return "Malejący";
            }
            else if (increasing)
            {
                return "Rosnący";
            }
            else
            {
                return "Nieokreślony";
            }
        }
    }
}

