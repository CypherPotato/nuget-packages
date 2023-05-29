using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion
{
    /// <summary>
    /// Provides an key-value collection for porting variables by Motion template views.
    /// </summary>
    /// <definition>
    /// public class VariableBag : IEnumerable
    /// </definition>
    /// <type>
    /// Class
    /// </type>
    public class VariableBag : IEnumerable<KeyValuePair<string, object?>>
    {
        private Dictionary<string, object?> _variables = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        public object? this[string name]
        {
            get => _variables[name];
            set
            {
                if (_variables.ContainsKey(name))
                {
                    _variables[name] = value;
                }
                else
                {
                    _variables.Add(name, value);
                }
            }
        }

        public int Count => _variables.Count;

        public void Add(string variableName, object? value)
        {
            _variables.Add(variableName, value);
        }

        public void Clear()
        {
            _variables.Clear();
        }

        public bool Contains(string variableName)
        {
            return _variables.ContainsKey(variableName);
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return _variables.GetEnumerator();
        }

        public bool Remove(string variableName)
        {
            return _variables.Remove(variableName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _variables.GetEnumerator();
        }
    }
}
