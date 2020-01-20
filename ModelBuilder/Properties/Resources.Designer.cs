﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModelBuilder.Properties {
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class Resources {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("ModelBuilder.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value cannot be null, white space or empty.
        /// </summary>
        internal static string ArgumentException_NullOrWhiteSpace {
            get {
                return ResourceManager.GetString("ArgumentException_NullOrWhiteSpace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} did not return an {1} instance. A build log is required to create an {2} instance.
        /// </summary>
        internal static string BuildStrategy_BuildLogRequired {
            get {
                return ResourceManager.GetString("BuildStrategy_BuildLogRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ConstructorResolver is null. A constructor resolver must be provided in order to create a build strategy.
        /// </summary>
        internal static string BuildStrategyCompiler_NullConstructorResolver {
            get {
                return ResourceManager.GetString("BuildStrategyCompiler_NullConstructorResolver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The PropertyResolver is null. A property resolver must be provided in order to create a build strategy.
        /// </summary>
        internal static string BuildStrategyCompiler_NullPropertyResolver {
            get {
                return ResourceManager.GetString("BuildStrategyCompiler_NullPropertyResolver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No public constructor is available for type {0}.
        /// </summary>
        internal static string ConstructorResolver_NoPublicConstructorFound {
            get {
                return ResourceManager.GetString("ConstructorResolver_NoPublicConstructorFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No valid constructors are available for type {0}.
        /// </summary>
        internal static string ConstructorResolver_NoValidConstructorFound {
            get {
                return ResourceManager.GetString("ConstructorResolver_NoValidConstructorFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Detected circular reference building type {0}, using previously created instance.
        /// </summary>
        internal static string DefaultBuildLog_CircularReferenceDetected {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CircularReferenceDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created parameter {0} ({1}) for type {2}.
        /// </summary>
        internal static string DefaultBuildLog_CreatedParameter {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatedParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Created property {0} ({1}) on type {2}.
        /// </summary>
        internal static string DefaultBuildLog_CreatedProperty {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatedProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End creating type {0}.
        /// </summary>
        internal static string DefaultBuildLog_CreatedType {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating parameter {0} ({1}) for type {2}.
        /// </summary>
        internal static string DefaultBuildLog_CreatingParameter {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatingParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating property {0} ({1}) on type {2}.
        /// </summary>
        internal static string DefaultBuildLog_CreatingProperty {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatingProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start creating type {0} using {1}.
        /// </summary>
        internal static string DefaultBuildLog_CreatingType {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatingType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating {0} value using {1}.
        /// </summary>
        internal static string DefaultBuildLog_CreatingValue {
            get {
                return ResourceManager.GetString("DefaultBuildLog_CreatingValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ignoring property {0} ({1}) on type {2}.
        /// </summary>
        internal static string DefaultBuildLog_IgnoringProperty {
            get {
                return ResourceManager.GetString("DefaultBuildLog_IgnoringProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mapped type requested from {0} to {1}.
        /// </summary>
        internal static string DefaultBuildLog_MappingType {
            get {
                return ResourceManager.GetString("DefaultBuildLog_MappingType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to End populating instance {0}.
        /// </summary>
        internal static string DefaultBuildLog_PopulatedInstance {
            get {
                return ResourceManager.GetString("DefaultBuildLog_PopulatedInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start populating instance {0}.
        /// </summary>
        internal static string DefaultBuildLog_PopulatingInstance {
            get {
                return ResourceManager.GetString("DefaultBuildLog_PopulatingInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Running PostBuild on {0} using {1}.
        /// </summary>
        internal static string DefaultBuildLog_PostBuild {
            get {
                return ResourceManager.GetString("DefaultBuildLog_PostBuild", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to determine type to build..
        /// </summary>
        internal static string DefaultBuildStrategy_UndeterminedTargetType {
            get {
                return ResourceManager.GetString("DefaultBuildStrategy_UndeterminedTargetType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No constructor found matching type..
        /// </summary>
        internal static string DefaultTypeCreator_NoMatchingConstructor {
            get {
                return ResourceManager.GetString("DefaultTypeCreator_NoMatchingConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expression &apos;{0}&apos; does not refer to a property.
        /// </summary>
        internal static string Error_ExpressionNotPropertyFormat {
            get {
                return ResourceManager.GetString("Error_ExpressionNotPropertyFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression returned a property {0} that was not defined on type &apos;{1}&apos;.
        /// </summary>
        internal static string Error_ExpressionTargetsWrongType {
            get {
                return ResourceManager.GetString("Error_ExpressionTargetsWrongType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not support the {1} type and reference name {2} in the context of the current build chain.
        /// </summary>
        internal static string Error_GenerationNotSupportedFormat {
            get {
                return ResourceManager.GetString("Error_GenerationNotSupportedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not support the {1} type.
        /// </summary>
        internal static string Error_TypeNotSupportedFormat {
            get {
                return ResourceManager.GetString("Error_TypeNotSupportedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The BuildChain property on the execution strategy is null.
        /// </summary>
        internal static string ExecuteStrategy_NoBuildChain {
            get {
                return ResourceManager.GetString("ExecuteStrategy_NoBuildChain", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos;.
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFound {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos; for &apos;{1}&apos;.
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFoundWithName {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFoundWithName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos; for &apos;{1}&apos; on type &apos;{2}&apos;.
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFoundWithNameAndContext {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFoundWithNameAndContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either declaringType, propertyType or propertyExpression must be supplied.
        /// </summary>
        internal static string NoOwnerTypePropertyTypeOrPropertyExpression {
            get {
                return ResourceManager.GetString("NoOwnerTypePropertyTypeOrPropertyExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either declaringType, propertyType or propertyName must be supplied.
        /// </summary>
        internal static string NoOwnerTypePropertyTypeOrPropertyName {
            get {
                return ResourceManager.GetString("NoOwnerTypePropertyTypeOrPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either propertyType or propertyExpression must be supplied.
        /// </summary>
        internal static string NoPropertyTypeOrPropertyExpression {
            get {
                return ResourceManager.GetString("NoPropertyTypeOrPropertyExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either propertyType or propertyName must be supplied.
        /// </summary>
        internal static string NoPropertyTypeOrPropertyName {
            get {
                return ResourceManager.GetString("NoPropertyTypeOrPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either targetType or propertyExpression must be supplied.
        /// </summary>
        internal static string NoTargetTypeOrPropertyExpression {
            get {
                return ResourceManager.GetString("NoTargetTypeOrPropertyExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either targetType or propertyName must be supplied.
        /// </summary>
        internal static string NoTargetTypeOrPropertyName {
            get {
                return ResourceManager.GetString("NoTargetTypeOrPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This generator was not created with a source expression. Call GetValue&lt;T&gt;(expression, context) instead.
        /// </summary>
        internal static string RelativeValueGenerator_NoSourceExpression {
            get {
                return ResourceManager.GetString("RelativeValueGenerator_NoSourceExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The rule {0} does not support type &apos;{1}&apos; for &apos;{2}&apos;.
        /// </summary>
        internal static string Rule_InvalidMatch {
            get {
                return ResourceManager.GetString("Rule_InvalidMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} is not assignable to {1}.
        /// </summary>
        internal static string TypeMappingRule_TypeNotAssignable {
            get {
                return ResourceManager.GetString("TypeMappingRule_TypeNotAssignable", resourceCulture);
            }
        }
    }
}
