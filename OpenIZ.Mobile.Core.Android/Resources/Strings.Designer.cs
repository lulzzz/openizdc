﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenIZ.Mobile.Core.Android.Resources {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OpenIZ.Mobile.Core.Android.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to General authentication error occurred.
        /// </summary>
        internal static string err_authentication_exception {
            get {
                return ResourceManager.GetString("err_authentication_exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified security certificate was not found.
        /// </summary>
        internal static string err_certificate_not_found {
            get {
                return ResourceManager.GetString("err_certificate_not_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Package is already installed.
        /// </summary>
        internal static string err_duplicate_package_name {
            get {
                return ResourceManager.GetString("err_duplicate_package_name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Accept header is not understood.
        /// </summary>
        internal static string err_invalid_accept {
            get {
                return ResourceManager.GetString("err_invalid_accept", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument is of wrong type.
        /// </summary>
        internal static string err_invalid_argumentType {
            get {
                return ResourceManager.GetString("err_invalid_argumentType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid Username/Password.
        /// </summary>
        internal static string err_login_invalidusername {
            get {
                return ResourceManager.GetString("err_login_invalidusername", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The authentication server cannot be contacted. Use the credentials you last used to access this device.
        /// </summary>
        internal static string err_offline_use_cache_creds {
            get {
                return ResourceManager.GetString("err_offline_use_cache_creds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid response received from server.
        /// </summary>
        internal static string err_response_failed_validation {
            get {
                return ResourceManager.GetString("err_response_failed_validation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Too many certificates matched the criteria (perhaps search criteria is too vague?).
        /// </summary>
        internal static string err_too_many_certificate_matches {
            get {
                return ResourceManager.GetString("err_too_many_certificate_matches", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot set context to unauthenticated principal.
        /// </summary>
        internal static string err_unauthenticated_principal {
            get {
                return ResourceManager.GetString("err_unauthenticated_principal", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        internal static string locale_cancel {
            get {
                return ResourceManager.GetString("locale_cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ok.
        /// </summary>
        internal static string locale_confirm {
            get {
                return ResourceManager.GetString("locale_confirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem with your device&apos;s configuration. The application has attempted to recover from this error and will restart..
        /// </summary>
        internal static string locale_restartRequired {
            get {
                return ResourceManager.GetString("locale_restartRequired", resourceCulture);
            }
        }
    }
}