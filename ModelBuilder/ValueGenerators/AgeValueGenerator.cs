﻿namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     The <see cref="AgeValueGenerator" />
    ///     class is used to generate numbers that should represent a persons age.
    /// </summary>
    public class AgeValueGenerator : RelativeValueGenerator, INullableBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressValueGenerator" /> class.
        /// </summary>
        public AgeValueGenerator() : base(NameExpression.Age)
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var generateType = type;

            if (generateType.IsNullable())
            {
                if (AllowNull)
                {
                    // Allow for a 10% the chance that this might be null
                    var range = Generator.NextValue(0, 100000);

                    if (range < 10000)
                    {
                        return null;
                    }
                }

                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                generateType = type.GetGenericArguments()[0];
            }

            var context = executeStrategy?.BuildChain?.Last;

            if (context == null)
            {
                return Generator.NextValue(generateType, MinAge, MaxAge);
            }

            // Check if there is a DOB value
            var dob = GetValue<DateTime>(NameExpression.DateOfBirth, context);

            if (dob == default)
            {
                return Generator.NextValue(generateType, MinAge, MaxAge);
            }

            // Calculate the age from this DOB
            var totalDays = DateTime.Now.Subtract(dob).TotalDays;

            if (totalDays > 0)
            {
                return Convert.ChangeType(Math.Floor(totalDays / 365), type, CultureInfo.CurrentCulture);
            }

            return Generator.NextValue(generateType, MinAge, MaxAge);
        }

        /// <inheritdoc />
        protected override bool IsMatch(IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                // This is neither a property nor a parameter
                return false;
            }

            if (NameExpression.Age.IsMatch(referenceName) == false)
            {
                return false;
            }

            if (type.IsNullable())
            {
                // Get the internal type
                var internalType = type.GetGenericArguments()[0];

                return Generator.IsSupported(internalType);
            }

            return Generator.IsSupported(type);
        }

        /// <inheritdoc />
        public bool AllowNull { get; set; } = false;

        /// <summary>
        ///     Gets or sets the maximum age generated by this instance.
        /// </summary>
        public int MaxAge { get; set; } = 100;

        /// <summary>
        ///     Gets or sets the minimum age generated by this instance.
        /// </summary>
        public int MinAge { get; set; } = 1;

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}