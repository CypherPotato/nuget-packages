using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    /// <summary>
    /// Provides the default Motion functions that can be called within view templates.
    /// </summary>
    /// <definition>
    /// public class MotionDefaultFunctions
    /// </definition>
    /// <type>
    /// Class
    /// </type>
    public class MotionDefaultFunctions
    {
        public string Include(MotionContext context, string includeName)
        {
            string? viewData = context.Engine.Views[includeName];

            if (viewData == null)
            {
                throw new ArgumentNullException(nameof(includeName));
            }

            return MotionEngine.RenderTemplate(viewData, context);
        }

        public string Upper(MotionContext context, object? expression)
        {
            return expression?.ToString().ToUpper() ?? "";
        }
    }
}
