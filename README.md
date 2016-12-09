# ModelBuilder
A library for easy generation of model classes

## Creating a model
The Model class is a static class that is the easiest way to generator models

```
var model = Model.Create<Person>();
```

This supports creating new instances of classes, nested classes and populating variables out of the box. It will also support providing constructor arguments to the top level type being created.

```
var model = Model.CreateWith<Person>("Fred", "Smith");
```

You may want to create a model that ignores setting a property for a specific construction.

```
var model = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();
```

Ignoring a property can also be configured for types that may exist deep in an inheritance hierarchy for the type being created.

```
var model = Model.Ignoring<Address>(x => x.AddressLine1).Create<Person>();
```

Do you already have an instance that perhaps you didn't create? That is ok too.

```
var person = new Person
{
    FirstName = "Jane"
};

var model = Model.Ignoring<Person>(x => x.FirstName).Populate<Person>();

var customer = new Person();

var customerModel = Model.Populate(customer);
```

### Constructor parameter matching

From version 3.0, ModelBuilder will attempt to match constructor parameters to property values when building a new instance to avoid a new random value overwriting the constructor value.

The following rules define how a match is made between a constructor parameter and a property value:

- No match if property type does not match the constructor parameter type (inheritance supported)
- No match if property value is the default value for its type
- Match reference types (except for strings) where the property value matches the same instance as the constructor parameter (using Object.ReferenceEquals)
- Match value types and strings that have the same value and the constructor parameter name is a case insensitive match to the property name

## Changing the model after creation

Sometimes you need to tweak a model after it has been created. This can be done easily using the Set extension method on any object.

```
var person = Model.Create<Person>().Set(x => x.FirstName = "Joe").Set(x => x.Email = null);

var otherPerson = Model.Create<Person>().Set(x => 
    {
        x.FirstName = "Joe";
        x.Email = null;
    });
```

This is nice for simple properties, but assigning values across an enumerable set of data is important. We've got that covered too.

```
var organisation = Model.Create<Organisation>();

organisation.Staff.SetEach(x => x.Email = null);
```

## Customizing the process

ModelBuilder is designed with extensibility in mind. There are many ways that you can customize the build configuration and the process of building models. 

The extensibility points for customizing the build configuration are:

- BuildStrategyCompiler
- CompilerModule
- BuildStrategy
- ExecuteStrategy

The extensibility points controlling how to create models are:

- TypeCreators
- ValueGenerators
- CreationRules
- IgnoreRules
- ExecuteOrderRules
- PostBuildActions
- ConstructorResolver

### BuildStrategyCompiler

A BuildStrategyCompiler provides the ability to define the configuration options of the above extensibility points and can compile a BuildStrategy. There is an inbuilt compiler that provides a default configuration to provide an out of the box BuildStrategy.
    
### CompilerModules
The behaviour of ModelBuilder is to create a BuildStrategy using a pre-defined BuildStrategyCompiler that includes a default configuration. It then scans for CompilerModules in the loaded assemblies to support additional configuration of the BuildStrategyCompiler before compiling a BuildStrategy.

Using a CompilerModule is the easiest mechanism of configuring the default BuildStrategy. Simply by creating a class that implements ICompilerModule will cause the class to be executed on the first call to Model.Create&lt;T&gt;.

```
public class MyCustomCompilerModule : ICompilerModule
{
    public void Configure(IBuildStrategyCompiler compiler)
    {
        compiler.Add(new MyCustomExecuteOrderRule());
        compiler.Add(new MyCustomIgnoreRule());
        compiler.Add(new MyCustomPostBuildAction());
        compiler.Add(new MyCustomCreationRule());
        compiler.Add(new MyCustomTypeCreator());
        compiler.Add(new MyCustomValueGenerator());
    }
}
```

### BuildStrategy 

A BuildStrategy contains the configuration of how to create models via all the above extesibility points. It exposes this configuration in a read-only manner. This ensures a consistent behaviour for creating models. BuildStrategy exposes an ExecuteStrategy instance via the GetExecuteStrategy&lt;T&gt; method.
    
Creating a model ultimately starts with a BuildStrategy configuration. There are a couple of options for building this configuration. 

You can create a custom BuildStrategy from scratch by creating your own type that implements IBuildStrategy. You can then assign this instance as the default BuildStrategy against Model which is then used in calls to Model.Create&lt;T&gt;.

```
Model.BuildStrategy = new CustomBuildStrategy();
```

The other option is to use a BuildStrategyCompiler which compiles a new BuildStrategy instance with a custom configuration. You can get a compiler from an existing BuildStrategy using the IBuildStrategy.Clone() extension method. A new BuildStrategy compiled from the BuildStrategyCompiler can then also be assigned against Model.BuildStrategy.

```
var strategy = ModelBuilder.DefaultBuildStrategy
    .Clone()
    .AddTypeCreator<MyCustomTypeCreator>()
    .AddValueGenerator<MyCustomValueGenerator>()
	.RemoveValueGenerator<EmailValueGenerator>()
	.AddValueGenerator<MailinatorEmailValueGenerator>()
	.AddCreationRule<Person>(x => x.IsAdministrator, false)
    .AddIgnoreRule<Person>(x => x.FirstName)
    .AddExecuteOrderRule<Person>(x => x.LastName, 10)
	.AddPostBuildAction<MyCustomPostBuildAction>()
    .Compile();

Model.BuildStrategy = strategy;
```

You may want to create multiple build strategies that support different model construction designs. These can be used on the fly as well.

```
var model = Model.Using<CustomBuildStrategy>().Create<Person>();
```

### ExecuteStrategy

ExecuteStrategy provides the logic that creates a model instance from the configuration in a provided BuildStrategy.
    
### Type creators

Type creators are used to create instances of classes, or reference types with the exception of System.String. There are two type creators that ModelBuilder provides out of the box. DefaultTypeCreator and EnumerableTypeCreator.

DefaultTypeCreator will create a new instance of a type using Activator.CreateInstance and supports the optional provision of constructor arguments.

EnumerableTypeCreate will create instances of most types derived from IEnumerable&lt;T&gt;. It will also attempt to populate the instance with data.

### Value generators

Value generators are used to create value types. There are many value generators that come out of the box. These are:

- AddressValueGenerator
- AgeValueGenerator
- BooleanValueGenerator
- CityValueGenerator
- CompanyValueGenerator
- CountryValueGenerator
- CountValueGenerator
- DateOfBirthValueGenerator
- DateTimeValueGenerator
- DomainNameValueGenerator
- EmailValueGenerator
- EnumValueGenerator
- FirstNameValueGenerator
- GenderValueGenerator
- GuidValueGenerator
- IPAddressValueGenerator
- LastNameValueGenerator
- MailinatorEmailValueGenerator **(NOTE: This generator is not included in DefaultBuildStrategy by default)**
- NumericValueGenerator
- PhoneValueGenerator
- PostCodeValueGenerator
- StateValueGenerator
- StringValueGenerator
- SuburbValueGenerator
- TimeZoneValueGenerator
- UriValueGenerator

Some of these generators create values using random data (string, guid, numbers etc). Entity type properties (names, addresses etc) use an embedded resource of 2,000 records as the data source to then pick pseudo-random information to provide better quality values.

### Creation rules

Creation rules are simple rules that bypass creating values using an IValueGenerator or an ITypeCreator. Most often they are used when creating a custom IBuildStrategy via BuildStrategyCompiler. Implementing a custom IValueGenerator or ITypeCreator is the preferred method if more complexity is required than simple creation rules (such as always making a property on a type a static value).

### Ignore rules

You may have a model with a property that you do not want to have a value generated for it. This can be done with an IgnoreRule.

### Execute order rules

Generating random or pseudo-random data for a model dynamically is never going to be perfect. We can however provide better data when some context is available.  ExecuteOrderRules help with this in that they define the order in which a property is assigned when the model is being created.

For example, if a Person type exposes a Gender property then the FirstName property should ideally match that gender. The DefaultBuildStrategy supports this by defining that the GenderValueGenerator gets executed before the FirstNameValueGenerator which is executed before the StringValueGenerator. Another example of this is that the DefaultBuildStrategy defines the enum properties will be assigned before other property types because they tend to be reference data that might define how other properties are assigned.

### Post-Build Actions

Post-build actions are much like the Set and SetEach extension methods. They provide the opportunity to tweak an instance after it has been created or populated. Suported post-build actions are evaluated in descending priority order after an instance has been created or populated.

### Constructor resolver

The constructor resolver is used to assist in resolving the constructor to execute when creating child instances of a model in the DefaultExecuteStrategy&lt;T&gt;. This supports a scenario where a property type to create is a type that does not have a default constructor and no parameters are available to create with. The constructor resolver will guess which is the most appropriate constructor to use for which the execute strategy will then create the required parameters.