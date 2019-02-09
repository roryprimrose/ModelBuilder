namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BooleanValueGenerator" />
    ///     class is used to generate random <see cref="bool" /> values.
    /// </summary>
    public class BooleanValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanValueGenerator" /> class.
        /// </summary>
        public BooleanValueGenerator()
            : base(typeof(bool), typeof(bool?))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == typeof(bool?))
            {
                var source = Generator.NextValue<double>(0, 3);

                bool? value;

                if (source < 1)
                {
                    value = false;
                }
                else if (source < 2)
                {
                    value = true;
                }
                else
                {
                    value = null;
                }

                return value;
            }

            var nextValue = Generator.NextValue(0, 1);

            if (nextValue == 0)
            {
                return false;
            }

            return true;
        }
    }
}