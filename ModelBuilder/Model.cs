namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Model"/>
    /// class provides the main entry point into generating model instances.
    /// </summary>
    public static class Model
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Create<T>()
        {
            return For<T>().Create();
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the default build and execute strategies and constructor arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        public static T CreateWith<T>(params object[] args)
        {
            return For<T>().CreateWith(args);
        }

        /// <summary>
        /// Returns a default execute strategy with a default build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create using the execute strategy.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static IExecuteStrategy<T> For<T>()
        {
            return With<DefaultExecuteStrategy<T>>();
        }

        /// <summary>
        /// Populates the properties of the specified instance using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Populate<T>(T instance)
        {
            return For<T>().Populate(instance);
        }

        /// <summary>
        /// Returns a new <see cref="IBuildStrategy"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of build strategy to create.</typeparam>
        /// <returns>A new build strategy.</returns>
        public static T Using<T>() where T : IBuildStrategy, new()
        {
            var strategy = new T();

            return strategy;
        }

        /// <summary>
        /// Returns a new execute strategy using <see cref="DefaultBuildStrategy"/>.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to create.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static T With<T>() where T : IExecuteStrategy, new()
        {
            return DefaultStrategy.With<T>();
        }

        /// <summary>
        /// Gets or sets the default build strategy to use in this application domain.
        /// </summary>
        public static IBuildStrategy DefaultStrategy { get; set; } = new DefaultBuildStrategy();
    }
}