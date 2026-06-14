namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="BuiltInValueSources" /> class
    ///     creates the default registry of value sources for the in-box leaf types: primitives, core
    ///     value types, and the common BCL types that appear in models.
    /// </summary>
    internal static class BuiltInValueSources
    {
        private const string StringAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // The inclusive human-plausible age range used by the Age and DateOfBirth named sources.
        private const int MinAge = 1;
        private const int MaxAge = 100;

        // Keys under which shared data is cached in the build scope so that related members of the same
        // instance stay internally consistent. Location-style members all read one cached location row,
        // and Age/DateOfBirth read one cached age, regardless of the order the members are built in.
        private const string LocationScopeKey = "ModelBuilder.BuiltIn.Location";
        private const string AgeScopeKey = "ModelBuilder.BuiltIn.Age";

        // The member-name aliases shared between the named-source registration (CreateNamedRegistry)
        // and the sibling lookups in NextEmail. Keeping them in one place ensures the email value
        // source reads the same member names the registry matches, so adding an alias cannot silently
        // break the email derivation for that spelling.
        private static readonly string[] _firstNameMembers = { "FirstName", "GivenName" };
        private static readonly string[] _lastNameMembers = { "LastName", "Surname", "FamilyName" };
        private static readonly string[] _domainMembers = { "Domain" };

        /// <summary>
        ///     Creates a new registry of entity-style value sources matched by member name.
        /// </summary>
        /// <returns>The populated registry.</returns>
        public static NamedValueSourceRegistry CreateNamedRegistry()
        {
            var registry = new NamedValueSourceRegistry();

            registry.Register(new DelegateValueSource<string>(NextFirstName), _firstNameMembers);
            registry.Register(new DelegateValueSource<string>(c => Pick(c, TestData.LastNames)), _lastNameMembers);
            registry.Register(new DelegateValueSource<string>(NextMiddleName), "MiddleName");
            registry.Register(new DelegateValueSource<string>(NextEmail), "Email");
            registry.Register(new DelegateValueSource<string>(c => Pick(c, TestData.Domains)), _domainMembers);
            registry.Register(new DelegateValueSource<string>(c => Pick(c, TestData.Companies)), "Company", "Business");
            registry.Register(new DelegateValueSource<string>(c => Location(c).Country), "Country");
            registry.Register(new DelegateValueSource<string>(c => Location(c).State), "State", "Province");
            registry.Register(new DelegateValueSource<string>(c => Location(c).City), "City", "Suburb", "Town");
            registry.Register(new DelegateValueSource<string>(c => Location(c).PostCode), "PostCode", "ZipCode", "Postcode", "Zip");
            registry.Register(new DelegateValueSource<string>(c => Location(c).Phone), "Phone", "Mobile", "Cell", "Fax");
            registry.Register(new DelegateValueSource<string>(c => Pick(c, TestData.TimeZones)), "TimeZone");

            // UserName is registered before the full-name source because member names are matched by
            // case-insensitive substring, first-registered wins. The full-name source matches the bare
            // alias "Name", which is a substring of "UserName", so UserName must be checked first.
            registry.Register(new DelegateValueSource<string>(NextUserName), "UserName", "Username", "Login");

            // The full-name source is registered last among the string sources. Its bare "Name" alias is a
            // substring of many member names (DomainName, CompanyName, CityName, and so on), so the more
            // specific sources above must be matched first. Anything still containing "Name" (or named
            // "FullName"/"DisplayName") falls through to a person-style full name here.
            registry.Register(new DelegateValueSource<string>(NextFullName), "FullName", "DisplayName", "Name");

            // Age and date-of-birth are constrained to a human-plausible range. A member named Age is an
            // int, but a random int would happily produce values like 100000 that make no sense for a
            // person, so a named source narrows it to MinAge..MaxAge. DateOfBirth derives a UTC date that
            // lands within the same age range. Members with these names that are NOT int/DateTime fall
            // through to the general value sources, because the named registry is keyed by both name and
            // type.
            registry.Register<int>(new DelegateValueSource<int>(NextAge), "Age");
            registry.Register<DateTime>(new DelegateValueSource<DateTime>(NextDateOfBirth), "DateOfBirth", "DOB", "BirthDate", "Birthday");

            return registry;
        }

        /// <summary>
        ///     Creates a new registry populated with all built-in value sources.
        /// </summary>
        /// <returns>The populated registry.</returns>
        public static ValueSourceRegistry CreateRegistry()
        {
            var registry = new ValueSourceRegistry();

            registry.Register<bool>(new DelegateValueSource<bool>(c => c.Random.NextBool()));
            registry.Register<byte>(new DelegateValueSource<byte>(c => c.Random.NextByte()));
            registry.Register<sbyte>(new DelegateValueSource<sbyte>(c => c.Random.NextSByte()));
            registry.Register<short>(new DelegateValueSource<short>(c => c.Random.NextInt16()));
            registry.Register<ushort>(new DelegateValueSource<ushort>(c => c.Random.NextUInt16()));
            registry.Register<int>(new DelegateValueSource<int>(c => c.Random.NextInt32()));
            registry.Register<uint>(new DelegateValueSource<uint>(c => c.Random.NextUInt32()));
            registry.Register<long>(new DelegateValueSource<long>(c => c.Random.NextInt64()));
            registry.Register<ulong>(new DelegateValueSource<ulong>(c => c.Random.NextUInt64()));
            registry.Register<float>(new DelegateValueSource<float>(c => c.Random.NextSingle()));
            registry.Register<double>(new DelegateValueSource<double>(c => c.Random.NextDouble()));
            registry.Register<decimal>(new DelegateValueSource<decimal>(c => c.Random.NextDecimal()));
            registry.Register<char>(new DelegateValueSource<char>(NextChar));
            registry.Register<string>(new DelegateValueSource<string>(NextString));
            registry.Register<Guid>(new DelegateValueSource<Guid>(NextGuid));

            registry.Register<DateTime>(new DelegateValueSource<DateTime>(NextDateTime));
            registry.Register<DateTimeOffset>(new DelegateValueSource<DateTimeOffset>(c => new DateTimeOffset(NextDateTime(c))));
            registry.Register<TimeSpan>(new DelegateValueSource<TimeSpan>(c => TimeSpan.FromMinutes(c.Random.NextInt32(0, 1440))));
            registry.Register<Uri>(new DelegateValueSource<Uri>(NextUri));
            registry.Register<Version>(new DelegateValueSource<Version>(NextVersion));
            registry.Register<byte[]>(new DelegateValueSource<byte[]>(NextBytes));

            return registry;
        }

        private static char NextChar(IBuildContext context)
        {
            return StringAlphabet[context.Random.NextInt32(0, StringAlphabet.Length - 1)];
        }

        private static string NextEmail(IBuildContext context)
        {
            // Prefer sibling name/domain values already set on the instance so the email is consistent
            // with them; otherwise compose a believable address from the data sets. The sibling lookups
            // use the same member-name aliases the registry matches, so a model that spells the members
            // GivenName/Surname (rather than FirstName/LastName) still derives a matching email.
            var first = context.GetSibling<string>(_firstNameMembers);
            var last = context.GetSibling<string>(_lastNameMembers);
            var domain = context.GetSibling<string>(_domainMembers);

            if (string.IsNullOrEmpty(first))
            {
                first = NextFirstName(context);
            }

            if (string.IsNullOrEmpty(last))
            {
                last = Pick(context, TestData.LastNames);
            }

            if (string.IsNullOrEmpty(domain))
            {
                domain = Pick(context, TestData.Domains);
            }

            return first!.ToLowerInvariant() + "." + last!.ToLowerInvariant() + "@" + domain;
        }

        private static string NextFirstName(IBuildContext context)
        {
            var useFemale = context.Random.NextBool();
            var names = useFemale ? TestData.FemaleNames : TestData.MaleNames;

            return Pick(context, names);
        }

        private static string NextMiddleName(IBuildContext context)
        {
            return NextFirstName(context);
        }

        private static string NextFullName(IBuildContext context)
        {
            // Compose from the first/last name siblings already set on the instance so the full name is
            // consistent with them; otherwise pick fresh values from the data sets.
            var first = context.GetSibling<string>(_firstNameMembers);
            var last = context.GetSibling<string>(_lastNameMembers);

            if (string.IsNullOrEmpty(first))
            {
                first = NextFirstName(context);
            }

            if (string.IsNullOrEmpty(last))
            {
                last = Pick(context, TestData.LastNames);
            }

            return first + " " + last;
        }

        private static string NextUserName(IBuildContext context)
        {
            // Derive a believable user name from the name siblings (consistent with FullName/Email),
            // with a numeric suffix to keep collisions unlikely across a generated set.
            var first = context.GetSibling<string>(_firstNameMembers);
            var last = context.GetSibling<string>(_lastNameMembers);

            if (string.IsNullOrEmpty(first))
            {
                first = NextFirstName(context);
            }

            if (string.IsNullOrEmpty(last))
            {
                last = Pick(context, TestData.LastNames);
            }

            return first!.ToLowerInvariant() + "." + last!.ToLowerInvariant() + context.Random.NextInt32(1, 99);
        }

        private static int NextAge(IBuildContext context)
        {
            // Cache one age for the instance being built so that Age and DateOfBirth stay consistent
            // regardless of which member is built first: saying Age is 18 while DateOfBirth implies 35 is
            // illogical. DateOfBirth reads this same cached age.
            return context.GetOrAddScopedValue(AgeScopeKey, c => c.Random.NextInt32(MinAge, MaxAge));
        }

        private static DateTime NextDateOfBirth(IBuildContext context)
        {
            // Anchor the birth date to the same fixed reference as NextDateTime and to the shared age so
            // the generated date represents exactly the cached age in completed years. Subtracting up to a
            // year of extra days keeps the completed-year count equal to the age.
            var ageYears = NextAge(context);
            var extraDays = context.Random.NextInt32(0, 364);

            return new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddYears(-ageYears).AddDays(-extraDays);
        }

        private static Location Location(IBuildContext context)
        {
            // Cache a single location row for the instance being built so that every location-style member
            // (Country, State, City, PostCode, Phone) is drawn from the same coherent row, avoiding
            // impossible combinations like "London, New South Wales, India". A different instance elsewhere
            // in the graph receives its own scope and therefore its own location.
            return context.GetOrAddScopedValue(LocationScopeKey, RandomLocation);
        }

        private static Location RandomLocation(IBuildContext context)
        {
            var locations = TestData.Locations;

            return locations[context.Random.NextInt32(0, locations.Count - 1)];
        }

        private static string Pick(IBuildContext context, IReadOnlyList<string> values)
        {
            if (values.Count == 0)
            {
                return string.Empty;
            }

            return values[context.Random.NextInt32(0, values.Count - 1)];
        }

        private static byte[] NextBytes(IBuildContext context)
        {
            var buffer = new byte[context.Random.NextInt32(1, 16)];

            context.Random.NextBytes(buffer);

            return buffer;
        }

        private static DateTime NextDateTime(IBuildContext context)
        {
            // A value within roughly the last ten years, kept well inside DateTime bounds.
            var ticksRange = TimeSpan.FromDays(3650).Ticks;
            var offset = context.Random.NextInt64(0, ticksRange);

            return new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(-offset);
        }

        private static Guid NextGuid(IBuildContext context)
        {
            var buffer = new byte[16];

            context.Random.NextBytes(buffer);

            return new Guid(buffer);
        }

        private static string NextString(IBuildContext context)
        {
            var length = context.Random.NextInt32(5, 12);
            var chars = new char[length];

            for (var index = 0; index < length; index++)
            {
                chars[index] = StringAlphabet[context.Random.NextInt32(0, StringAlphabet.Length - 1)];
            }

            return new string(chars);
        }

        private static Uri NextUri(IBuildContext context)
        {
            // Build the URI from the curated domain data set (the same source NextEmail uses for the
            // email domain) so generated URIs read like real hosts and stay consistent with emails.
            var domain = Pick(context, TestData.Domains);

            if (string.IsNullOrEmpty(domain))
            {
                domain = NextString(context).ToLowerInvariant() + ".example.com";
            }

            return new Uri("https://" + domain + "/");
        }

        private static Version NextVersion(IBuildContext context)
        {
            return new Version(
                context.Random.NextInt32(0, 20),
                context.Random.NextInt32(0, 20),
                context.Random.NextInt32(0, 99),
                context.Random.NextInt32(0, 9999));
        }

        /// <summary>
        ///     Gets the shared default registry of built-in value sources.
        /// </summary>
        public static ValueSourceRegistry Default { get; } = CreateRegistry();

        /// <summary>
        ///     Gets the shared default registry of entity-style value sources matched by member name.
        /// </summary>
        public static NamedValueSourceRegistry DefaultNamed { get; } = CreateNamedRegistry();
    }
}
