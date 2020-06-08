namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The <see cref="FactoryTypeCreator"/>
    /// class is used to create a value using a static factory method found on the type.
    /// </summary>
    public class FactoryTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
            string? referenceName)
        {
            var baseValue = base.CanCreate(configuration, buildChain, type, referenceName);

            if (baseValue == false)
            {
                return false;
            }

            // Check if there is no constructor to use
            var constructor = configuration.ConstructorResolver.Resolve(type);

            if (constructor != null)
            {
                // There is a valid constructor to use so we don't need to search for a factory method
                return false;
            }

            var method = GetFactoryMethod(type);

            if (method == null)
            {
                // There is no factory method that can be used
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            var method = GetFactoryMethod(type);

            if (method == null)
            {
                throw new InvalidOperationException($"Failed to resolve factory method on type '{type.FullName}'");
            }

            var parameters = method.GetParameters();

            if (parameters == null)
            {
                return method.Invoke(null, null);
            }

            var parameterArguments = new object[parameters.Length];

            var lastContext = executeStrategy.BuildChain.Last;

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index]!;

                executeStrategy.Log.CreatingParameter(parameter, lastContext);

                var value = executeStrategy.Create(parameter.ParameterType);

                parameterArguments[index] = value;

                executeStrategy.Log.CreatedParameter(parameter, lastContext);
            }

            return method.Invoke(null, parameterArguments);
        }

        /// <inheritdoc />
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotImplementedException();
        }

        private static MethodInfo? GetFactoryMethod(Type type)
        {
            var bindingFlags = BindingFlags.Static
                               | BindingFlags.FlattenHierarchy
                               | BindingFlags.Public
                               | BindingFlags.InvokeMethod;

            // Get all the public static methods that return the return type but do not have the return type as a parameter
            // order by the the methods with the least amount of parameters
            var methods = from x in type.GetMethods(bindingFlags)
                orderby x.GetParameters().Length
                where type.IsAssignableFrom(x.ReturnType)
                      && x.GetParameters().Any(y => y.ParameterType == type) == false
                select x;

            return methods.FirstOrDefault();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 200;
    }
}