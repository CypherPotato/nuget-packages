using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    /// <summary>
    /// Provides an class which holds context for rendering view templates with <see cref="MotionEngine"/>.
    /// </summary>
    /// <definition>
    /// public class MotionContext
    /// </definition>
    /// <type>
    /// Class
    /// </type>
    public class MotionContext
    {
        /// <summary>
        /// Creates an <see cref="MotionContext"/> instance with default parameters.
        /// </summary>
        /// <param name="bag">Optional. The variable bag which will set this motion context.</param>
        /// <returns></returns>
        /// <definition>
        /// public static MotionContext CreateDefault(VariableBag? bag = null)
        /// </definition>
        /// <type>
        /// Static method
        /// </type>
        public static MotionContext CreateDefault(VariableBag? bag = null)
        {
            return new MotionContext(bag, new MotionEngine()
            {
                Functions = new MotionDefaultFunctions()
            });
        }

        /// <summary>
        /// Gets or sets the <see cref="VariableBag"/> for this Motion context.
        /// </summary>
        /// <definition>
        /// public VariableBag? Bag { get; set; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public VariableBag? Bag { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MotionEngine"/> for this Motion context.
        /// </summary>
        /// <definition>
        /// public MotionEngine Engine { get; set; }
        /// </definition>
        /// <type>
        /// Property
        /// </type>
        public MotionEngine Engine { get; set; }

        /// <summary>
        /// Creates an new <see cref="MotionContext"/> instance with given parameters.
        /// </summary>
        /// <param name="bag">The <see cref="VariableBag"/> for this Motion context.</param>
        /// <param name="eng">The <see cref="MotionEngine"/> for this Motion context.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <definition>
        /// public MotionContext(VariableBag? bag, MotionEngine eng)
        /// </definition>
        /// <type>
        /// Constructor
        /// </type>
        public MotionContext(VariableBag? bag, MotionEngine eng)
        {
            Bag = bag;
            Engine = eng ?? throw new ArgumentNullException(nameof(eng));
        }
    }
}
