# ModelBuilder
A library for easy generation of model classes

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/ModelBuilder/blob/master/LICENSE)&nbsp;[![Nuget](https://img.shields.io/nuget/v/ModelBuilder.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/ModelBuilder.svg)](https://www.nuget.org/packages/ModelBuilder)

[![Actions Status](https://github.com/roryprimrose/ModelBuilder/workflows/CI/badge.svg)](https://github.com/roryprimrose/ModelBuilder/actions)&nbsp;[![Coverage Status](https://coveralls.io/repos/github/roryprimrose/ModelBuilder/badge.svg?branch=master)](https://coveralls.io/github/roryprimrose/ModelBuilder?branch=master)

## Creating a model
The Model class is a static class that is the easiest way to generator models

```
var model = Model.Create<Person>();
```

This supports creating new instances of classes, nested classes and populating variables out of the box. It will also support providing constructor arguments to the top level type being created.

```
var model = Model.Create<Person>("Fred", "Smith");
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

ModelBuilder will attempt to match constructor parameters to property values when building a new instance to avoid a new random value overwriting the constructor value.

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

- IBuildConfiguration
- IConfigurationModule
- IExecuteStrategy

The extensibility points defined in `IBuildConfiguration` that are used to control how to create models are:

- IConstructorResolver
- ICreationRule
- IExecuteOrderRule
- IIgnoreRule
- IPostBuildAction
- IPropertyResolver
- ITypeCreator
- TypeMappingRule
- ITypeResolver
- IValueGenerator

### IBuildConfiguration

An `IBuildConfiguration` instance provides access to all the configuration used to create values. The `Model` class uses a default configuration that is built using the `DefaultConfigurationModule`. You could however start with an empty `BuildConfiguration` and modify the configuration from there.   

```
var configuration = new BuildConfiguration();

configuration.Add(new MyCustomTypeCreator());

var value = configuration.Create<MyType>();
```

The items held by a build configuration can be modified for specific scenarios. For example, generating an Age value using the `AgeValueGeneartor` creates an age between 1 and 100. This value generation behavior can be modified.

```
var model = Model.UsingDefaultConfiguration()
    .UpdateValueGenerator<AgeValueGenerator>(x => x.MinAge = 18)
    .Create<Person>();
```

### IConfigurationModule
An `IConfigurationModule` defines a reusable configuration definition that is applied to `IBuildConfiguration`. You can use the `DefaultConfigurationModule` or write your own by creating a class that implements IConfigurationModule.

```
public class MyCustomModule : IConfigurationModule
{
    public void Configure(IBuildConfiguration configuration)
    {
        compiler.AddCreationRule<MyCreationRule>();
        compiler.AddExecuteOrderRule<MyExecuteOrderRule>();
        compiler.AddIgnoreRule<MyIgnoreRule>();
        compiler.AddPostBuildAction<MyPostBuildAction>();
        compiler.AddTypeCreator<MyTypeCreator>();
        compiler.AddTypeMappingRule<MyTypeMappingRule>();
        compiler.AddValueGenerator<MyValueGenerator>();
		
        // Or
        compiler.Add(new MyCreationRule());
        compiler.Add(new MyExecuteOrderRule());
        compiler.Add(new MyIgnoreRule());
        compiler.Add(new MyPostBuildAction());
        compiler.Add(new MyTypeCreator());
        compiler.Add(new MyTypeMappingRule());
        compiler.Add(new MyValueGenerator());
    }
}
```

```
var model = Model.UsingModule<MyCustomModule>().Create<Person>();
```

You may want to create multiple compiler modules that support different model construction designs. 

```
var model = Model.UsingModule<MyUnitTestModule>().Create<Person>();
var otherModel = Model.UsingModule<MyIntegrationTestModule>().Create<Person>();
```

You can also daisy-chain modules to build on top of other modules.

```
var model = Model.UsingDefaultConfiguration
    .UsingModule<MyCustomModule>()
    .UsingModule<MyOtherModule>()
    .Create<Person>();
```

The configuration modules are useful when you want to provide some consistent defaults for creating values. For example, you may have an Account entity with an IsActive boolean property and those should always be created with a value of `true` rather than a random value.

```
public class TestModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(IBuildConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.AddCreationRule<Account>(x => x.IsActive, 100, true)
    }
}
```

You can then use this common build configuration across unit tests by referencing the module.

```
[Fact]
public void GetReturnsAccountWhenActiveIsTrue()
{
    var account = Model.UsingModule<TestModule>.Create<Account>();

    // account.IsActive will always be true
}
```

### IExecuteStrategy

An `IExecuteStrategy` provides the logic that creates a model instance from the configuration in a provided `IBuildConfiguration`.
    
### IConstructorResolver

An `IConstructorResolver` is used to assist in resolving the constructor to execute when creating instances of a model. 

The `DefaultConstructorResolver` provides the logic for selecting the constructor with the least number of parameters unless constructor arguments have been supplied. In that case it selects a constructor that matches the argument list.

### ICreationRule

An `ICreationRule` provides a simple implementation for returning a model value that bypasses the complexity of creating values using an IValueGenerator or an ITypeCreator. Most often they are used when creating a custom `IBuildConfiguration`. 

For example:

```
public class TestModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(IBuildConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.AddCreationRule(typeof(bool), "^IsActive$", true, 100); // Make all boolean IsActive properties true across all types
        configuration.AddCreationRule<Account>(x => x.IsReseller, false, 100);
        configuration.AddCreationRule<MyCustomCreationRule>();
    }
}
```

Implementing a custom IValueGenerator or ITypeCreator is the preferred method if more complexity is required than assigning simple values.

### IExecuteOrderRule

Generating random or pseudo-random data for a model dynamically is never going to be perfect. We can however provide better data when some context is available.  An `IExecuteOrderRule` helps here by defining the order in which a property or parameter value is created when the model is being created. This means that creating one value may then be dependent on another by controlling the order in which they are created.

For example, if a Person type exposes an Email, FirstName and LastName properties then the email value should include the first and last names. The `DefaultConfigurationModule` supports this by defining that the execute order for this scenario is FirstName, LastName, Domain, Email and then other string properties or parameters. 

```
configuration.AddExecuteOrderRule(NameExpression.FirstName, 9580);
configuration.AddExecuteOrderRule(NameExpression.LastName, 9560);
configuration.AddExecuteOrderRule(NameExpression.Domain, 9550);
configuration.AddExecuteOrderRule(NameExpression.Email, 9540);

configuration.AddExecuteOrderRule(x => x.PropertyType == typeof(string), 2000);
configuration.AddExecuteOrderRule(x => x.PropertyType.IsClass, 1000);
```

Another example of this is that the default configuration defines the enum properties will be assigned before other property types because they tend to be reference data that might define how other properties are assigned.

```
configuration.AddExecuteOrderRule(x => x.PropertyType.IsEnum, 4000);
configuration.AddExecuteOrderRule(x => x.PropertyType.IsValueType, 3000);
configuration.AddExecuteOrderRule(x => x.PropertyType == typeof(string), 2000);
configuration.AddExecuteOrderRule(x => x.PropertyType.IsClass, 1000);
```

### IIgnoreRule

An `IIgnoreRule` identifies a property that should not have a value generated for it. 

Ignore rules can be added to a `IConfigurationModule` so that the configuration is reusable.

```
public class TestModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(IBuildConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.AddIgnoreRule<Request>(x => x.Data);
    }
}
```

If ignoring a property is specific to a test scenario that this could also be inlined in the create call.

```
var model = Model.Ignoring<PersonalDetails>(x => x.Age).Create<CreateAccountRequest>();
```

### IPostBuildAction

An `IPostBuildAction` are like the `Set` and `SetEach` extension methods. They provide the opportunity to tweak an instance after it has been created or populated. Suported post-build actions are evaluated in descending priority order after an instance has been created or populated.

### IPropertyResolver

An `IPropertyResolver` is used to assist in resolving the properties on a type when populating instances of a model. 

The `DefaultPropertyResolver` provides the logic for identifying the properties on a type that can be populated and the order in which they should be populated (using `IExecuteOrderRule` values). It also supports identifying whether a property should be populated even though it can be. The typcial scenario here is that a property that returns a value provided in a constructor parameter should not be overwritten with a new value.

### ITypeCreator

An `ITypeCreator` is used to create instances of classes, or reference types with the exception of System.String. There are three type creators that ModelBuilder provides out of the box. EnumerableTypeCreator, ArrayTypeCreator and DefaultTypeCreator.

`ArrayTypeCreate` will create instances of Array types and populates the instance with data.

`EnumerableTypeCreate` will create instances of most types derived from `IEnumerable<T>` and populates the instance with data.

`DefaultTypeCreator` will create a new instance of any type using `Activator.CreateInstance` and supports the optional provision of constructor arguments. Any parameters required by the constructor will also be created.

### TypeMappingRule

A `TypeMappingRule` can identify a mapping between types when one needs to be created. For example, a `Stream` property on a type can not be created as it is an abstract type. A type mapping can identify the type of `Stream` to create however.

This can be defined in a configuration module.

```
public class TestModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(IBuildConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration.AddTypeMappingRule<Stream, MemoryStream>();
    }
}
```

A type mapping could also be inlined.

```
var model = Model.Mapping<Stream, MemoryStream>().Create<RequestPayload>();
```

### ITypeResolver

An `ITypeResolver` identifies a type to create. 

The `DefaultTypeResolver` provides some automatic detection of types to create when the type requested is an interfaces or abstract type. This is helpful for testing code that uses interfaces and abstract classes for models and their properties/parameters. It reduces the number of times that a manual type mapping will be required however results may be non-deterministic.

The `DefaultTypeResolver` will scan all the types in the assembly that defines the requested type. It will try to create an object using the first matching type it identifies. 

For example, you may have a class that returns an interface property that is not already supported by a `ITypeCreator` or `TypeMappingRule`.

```
public interface IManifest
{
    bool IsCurrent();

    DateTimeOffset Sent { get; set; }

    DateTimeOffset ExpectedAt { get; set; }
}

public class Manifest : IManifest
{
    public bool IsCurrent()
    {
        var now = DateTimeOffset.UtcNow;

        return (Sent < now && ExpectedAt > now);
    }

    public DateTimeOffset Sent { get; set; }

    public DateTimeOffset ExpectedAt { get; set; }
}

public class ShippingContainer
{
    public IManifest Manifest { get; set; }
}
```

The ShippingContainer model here can then be created without an specific type mapping.
```
Model.Create<ShippingContainer>()
```

### IValueGenerator

An `IValueGenerator` is used to create value types. There are many value generators that come out of the box. These are:

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
- TimeZoneInfoValueGenerator
- TimeZoneValueGenerator
- UriValueGenerator

Some of these generators create values using random data (string, guid, numbers etc). Entity type properties (names, addresses etc) use an embedded resource data to then pick pseudo-random information to provide better quality values.

## Upgrading to 6.0.0

The package had some large design changes that introduce breaking changes to the API if you were customising the model creation process. The following changes have been made.

- All ValueGenerator types have been moved into a ValueGenerators namespace.
- All TypeCreator types have been moved into a TypeCreators namespace.
- CreationRule has been replaced with ExpressionCreationRule, PredicateCreationRule and RegexCreationRule which are in the CreationRules namespace.
- The combination of build strategy, compiler and configuration have been replaced with just `IBuildConfiguration`.
- IBuildConfiguration is now mutable. A new IBuildConfiguration is created for each call to a static method on the `Model` class however that some configuration is used for the entire creation process. Any mutations to the build configuration will apply until the entire build tree has completed.
- `ICompilerModule` has been renamed to `IConfigurationModule` and now configures `IBuildConfiguration`.
- The `IgnoreRule` class has been replaced with the `IIgnoreRule` interface. The logic for processing rule matches moves from `DefaultPropertyResolver` to the rule itself.
- Renamed IValueGenerator.IsSupported to IsMatch
- Renamed IPostBuildAction.IsSupported to IsMatch
- Added IBuildConfiguration.TypeResolver
- Added IBuildConfiguration parameter to ITypeCreator.CanCreate
- ICreationRule, ITypeCreator, IValueGenerator, IPostBuildAction and IBuildLog now have overloads for ParameterInfo, PropertyInfo and Type rather than (Type, string)
- IExecuteOrderRule and IIgnoreRule now have overloads for PropertyInfo rather than (Type, string)
- RelativeValueGenerator no longer supports targeting a specific property as defined by the constructor. Getting a value from another property is now only available with an explicit call to `GetPropertyValue`.