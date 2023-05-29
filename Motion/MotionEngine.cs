using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    /// <summary>
    /// Provides an template-engine to interpolate strings in an advanced way.
    /// </summary>
    /// <definition>
    /// public class MotionEngine
    /// </definition>
    /// <type>
    /// Class
    /// </type>
    public class MotionEngine
    {
        /// <summary>
        /// Gets or sets the views templates.
        /// </summary>
        /// <definition>
        /// public NameValueCollection Views { get; set; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public NameValueCollection Views { get; set; }

        /// <summary>
        /// Gets or sets the object instance which contains the functions which will be called by the Motion Engine.
        /// </summary>
        /// <definition>
        /// public object? Functions { get; set; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public object? Functions { get; set; }

        /// <summary>
        /// Initializes an new <see cref="MotionEngine"/> instance.
        /// </summary>
        /// <definition>
        /// public MotionEngine()
        /// </definition>
        /// <type>
        /// Constructor
        /// </type>
        public MotionEngine()
        {
            Views = new NameValueCollection();
        }

        /// <summary>
        /// Renders an view template by their name.
        /// </summary>
        /// <param name="name">The view template name.</param>
        /// <definition>
        /// public string Render(string name)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public string Render(string name) => Render(name, null);

        /// <summary>
        /// Renders an view template by their name with an variable bag.
        /// </summary>
        /// <param name="name">The view template name.</param>
        /// <param name="bag">Variables to pass to the view rendering.</param>
        /// <definition>
        /// public string Render(string name, VariableBag? bag)
        /// </definition>
        /// <type>
        /// Method
        /// </type>
        public string Render(string name, VariableBag? bag)
        {
            string? viewData = Views[name];

            if (viewData == null)
            {
                throw new Exception("View not found: " + name);
            }

            var context = new MotionContext(bag, this);
            return RenderTemplate(viewData, context);
        }

        private static string InternalExpr(string expr, MotionContext context)
        {
            expr = expr.Trim() + " ";

            if (context.Engine.Functions == null)
            {
                throw new InvalidOperationException("Engine functions was not defined.");
            }

            bool hasCommand = true;
            int commandIndex = expr.IndexOf(' ');
            string command = expr.Substring(0, commandIndex);
            string rest = expr.Substring(commandIndex + 1);

            if (string.IsNullOrEmpty(rest))
            {
                hasCommand = false;
                rest = command + " ";
            }

            bool anyCharWritted = false;
            bool nextExpressionIsLiteral = false;
            bool inString = false;
            char before = '\0';
            StringBuilder expression = new StringBuilder();

            List<object?> parameters = new List<object?>()
            {
                context
            };
            List<Type> paramsTypes = new List<Type>()
            {
                typeof(MotionContext)
            };

            for (int i = 0; i < rest.Length; i++)
            {
                char ch = rest[i];

                if (ch == '"' && before != '\\')
                {
                    if (!anyCharWritted)
                    {
                        nextExpressionIsLiteral = true;
                        anyCharWritted = true;
                    }

                    inString = !inString;
                    continue;
                }
                else if (!inString && ch == ' ')
                {
                    string exp = expression.ToString();

                    if (string.IsNullOrWhiteSpace(exp))
                    {
                        continue;
                    }
                    if (nextExpressionIsLiteral)
                    {
                        parameters.Add(exp);
                        paramsTypes.Add(typeof(string));
                    }
                    else if (String.Compare(exp, "True", true) == 0)
                    {
                        parameters.Add(true);
                        paramsTypes.Add(typeof(bool));
                    }
                    else if (String.Compare(exp, "False", true) == 0)
                    {
                        parameters.Add(false);
                        paramsTypes.Add(typeof(bool));
                    }
                    else if (exp.All(c => char.IsDigit(c)))
                    {
                        int num = Int32.Parse(exp);
                        parameters.Add(num);
                        paramsTypes.Add(typeof(int));
                    }
                    else if (exp.All(c => char.IsDigit(c) || c == '.'))
                    {
                        double num = double.Parse(exp);
                        parameters.Add(num);
                        paramsTypes.Add(typeof(double));
                    }
                    else if (exp.Contains('='))
                    {
                        string trimmed = exp.Trim();
                        int ind = trimmed.IndexOf('=');
                        string varName = trimmed.Substring(0, ind);
                        string value = trimmed.Substring(ind + 1);

                        if (context.Bag == null)
                        {
                            context.Bag = new VariableBag();
                        }

                        context.Bag[varName] = value;
                    }
                    else
                    {
                        if (context.Bag?.Contains(exp) == true)
                        {
                            var obj = context.Bag[exp];
                            parameters.Add(obj);
                            paramsTypes.Add(obj?.GetType() ?? typeof(object));
                        }
                        else
                        {
                            throw new Exception("Invalid expression or unrecognized variable: " + exp);
                        }
                    }
                    anyCharWritted = false;
                    nextExpressionIsLiteral = false;
                    expression.Clear();
                }

                if (!char.IsWhiteSpace(ch))
                {
                    anyCharWritted = true;
                }

                if (anyCharWritted)
                    expression.Append(ch);
                before = ch;
            }

            string? result;
            if (hasCommand)
            {
                MethodInfo? commandMethod = context.Engine.Functions
                   .GetType()
                   .GetMethod(command, BindingFlags.IgnoreCase
                                     | BindingFlags.Instance
                                     | BindingFlags.Public
                                     | BindingFlags.NonPublic,
                                     paramsTypes.ToArray());

                if (commandMethod == null)
                {
                    string commandLiteral = command;
                    foreach (Type t in paramsTypes.Skip(1))
                    {
                        commandLiteral += " " + t.Name;
                    }
                    throw new Exception("Command {" + commandLiteral + "} is not defined.");
                }

                result = (string?)commandMethod.Invoke(context.Engine.Functions, parameters.ToArray());
            }
            else
            {
                result = parameters[1]?.ToString();
            }

            return result ?? "";
        }

        /// <summary>
        /// Renders an raw template literal with an predefined context.
        /// </summary>
        /// <param name="template">The view template contents.</param>
        /// <param name="context">The <see cref="MotionContext"/> object to render the view.</param>
        /// <definition>
        /// public static string RenderViewData(string viewData, MotionContext context)
        /// </definition>
        /// <type>
        /// Static method
        /// </type>
        public static string RenderTemplate(string template, MotionContext context)
        {
            StringBuilder finalOutput = new StringBuilder();
            StringBuilder parsingExpression = new StringBuilder();

            int expressionIndex = 0;
            for (int charPos = 0; charPos < template.Length; charPos++)
            {
                char ch = template[charPos];

                if (ch == '@')
                {
                    expressionIndex = 1;
                    continue;
                }
                if (expressionIndex == 1 && ch == '{')
                {
                    expressionIndex = 2;
                    continue;
                }
                if (expressionIndex == 2 && ch == '}')
                {
                    expressionIndex = 0;
                    string result = InternalExpr(parsingExpression.ToString(), context);
                    finalOutput.Append(result);
                    parsingExpression.Clear();
                    continue;
                }
                else if (expressionIndex == 1)
                {
                    expressionIndex = 0;
                }

                if (expressionIndex != 2)
                {
                    finalOutput.Append(ch);
                }
                else
                {
                    parsingExpression.Append(ch);
                }
            }

            return finalOutput.ToString();
        }
    }
}
