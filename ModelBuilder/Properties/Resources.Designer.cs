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
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ModelBuilder.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No public constructor is available for type {0}..
        /// </summary>
        internal static string ConstructorResolver_NoPublicConstructorFound {
            get {
                return ResourceManager.GetString("ConstructorResolver_NoPublicConstructorFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expression &apos;{0}&apos; does not refer to a property..
        /// </summary>
        internal static string Error_ExpressionNotPropertyFormat {
            get {
                return ResourceManager.GetString("Error_ExpressionNotPropertyFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} does not support the {1} type..
        /// </summary>
        internal static string Error_TypeNotSupportedFormat {
            get {
                return ResourceManager.GetString("Error_TypeNotSupportedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Either targetType or propertyName must be supplied..
        /// </summary>
        internal static string ExecuteOrderRule_NoTargetTypeOrPropetyName {
            get {
                return ResourceManager.GetString("ExecuteOrderRule_NoTargetTypeOrPropetyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression returned a property {0} that was not defined on type &apos;{1}&apos;..
        /// </summary>
        internal static string ExecuteStrategy_ExpressionTargetsWrongType {
            get {
                return ResourceManager.GetString("ExecuteStrategy_ExpressionTargetsWrongType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos;..
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFound {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos; for &apos;{1}&apos;..
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFoundWithName {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFoundWithName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No supporting ITypeCreator or IValueGenerator was found to build the type &apos;{0}&apos; for &apos;{1}&apos; on type &apos;{2}&apos;..
        /// </summary>
        internal static string NoMatchingCreatorOrGeneratorFoundWithNameAndContext {
            get {
                return ResourceManager.GetString("NoMatchingCreatorOrGeneratorFoundWithNameAndContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&apos;1.0&apos; encoding=&apos;UTF-8&apos;?&gt;&lt;DataSet&gt;&lt;Person&gt;&lt;Gender&gt;Female&lt;/Gender&gt;&lt;FirstName&gt;Patricia&lt;/FirstName&gt;&lt;LastName&gt;Sims&lt;/LastName&gt;&lt;Email&gt;psims0@google.co.uk&lt;/Email&gt;&lt;Company&gt;Brainlounge&lt;/Company&gt;&lt;DomainName&gt;ucsd.edu&lt;/DomainName&gt;&lt;Address&gt;7041 Swallow Avenue&lt;/Address&gt;&lt;State/&gt;&lt;PostCode/&gt;&lt;City&gt;Joyabaj&lt;/City&gt;&lt;Phone&gt;502-(259)264-0579&lt;/Phone&gt;&lt;Country&gt;Guatemala&lt;/Country&gt;&lt;TimeZone&gt;Africa/Harare&lt;/TimeZone&gt;&lt;/Person&gt;&lt;Person&gt;&lt;Gender&gt;Male&lt;/Gender&gt;&lt;FirstName&gt;Keith&lt;/FirstName&gt;&lt;LastName&gt;Berry&lt;/LastName&gt;&lt;Email&gt;kberry1@eba [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string People {
            get {
                return ResourceManager.GetString("People", resourceCulture);
            }
        }
    }
}
