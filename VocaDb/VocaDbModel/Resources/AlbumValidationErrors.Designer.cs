﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VocaDb.Model.Resources {
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
    internal class AlbumValidationErrors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AlbumValidationErrors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VocaDb.Model.Resources.AlbumValidationErrors", typeof(AlbumValidationErrors).Assembly);
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
        ///   Looks up a localized string similar to Album needs at least one artist..
        /// </summary>
        internal static string NeedArtist {
            get {
                return ResourceManager.GetString("NeedArtist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Release year needs to be specified..
        /// </summary>
        internal static string NeedReleaseYear {
            get {
                return ResourceManager.GetString("NeedReleaseYear", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Album needs a tracklist..
        /// </summary>
        internal static string NeedTracks {
            get {
                return ResourceManager.GetString("NeedTracks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Album type needs to be specified..
        /// </summary>
        internal static string NeedType {
            get {
                return ResourceManager.GetString("NeedType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Album needs at least one entry whose language isn&apos;t &apos;Unspecified&apos;..
        /// </summary>
        internal static string UnspecifiedNames {
            get {
                return ResourceManager.GetString("UnspecifiedNames", resourceCulture);
            }
        }
    }
}
