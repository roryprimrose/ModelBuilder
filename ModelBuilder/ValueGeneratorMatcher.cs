using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    public abstract class ValueGeneratorMatcher : ValueGeneratorBase
    {
        private readonly Func<Type, string, object, bool> _matcher;

        protected ValueGeneratorMatcher(params Type[] types)
        {
            _matcher = (type, referenceName, context) =>
            {
                return types.Any(x => x == type);
            };
        }

        protected ValueGeneratorMatcher(string referenceName, params Type[] types)
        {
            if (referenceName == null)
            {
                throw new ArgumentNullException(nameof(referenceName));
            }

            _matcher = (type, name, context) =>
            {
                var matches = from x in types
                    where x == type
                          && referenceName.Equals(name, StringComparison.OrdinalIgnoreCase)
                    select x;

                return matches.Any();
            };
        }

        protected ValueGeneratorMatcher(Regex expression, params Type[] types)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            _matcher = (type, name, context) =>
            {
                if (name == null)
                {
                    // We can't match on null names with Regex so it can be a match
                    return false;
                }

                var matches = from x in types
                              where x == type
                                    && expression.IsMatch(name)
                              select x;

                return matches.Any();
            };
        }

        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return _matcher(type, referenceName, context);
        }
    }
}