﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ViewRes {
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
    public class MiscStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MiscStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VocaDb.Web.Resources.Views.Shared.MiscStrings", typeof(MiscStrings).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to album(s).
        /// </summary>
        public static string AlbumCount {
            get {
                return ResourceManager.GetString("AlbumCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to artist(s).
        /// </summary>
        public static string ArtistCount {
            get {
                return ResourceManager.GetString("ArtistCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Event series.
        /// </summary>
        public static string EventSeries {
            get {
                return ResourceManager.GetString("EventSeries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to times.
        /// </summary>
        public static string FavoritedTimes {
            get {
                return ResourceManager.GetString("FavoritedTimes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to song(s).
        /// </summary>
        public static string SongCount {
            get {
                return ResourceManager.GetString("SongCount", resourceCulture);
            }
        }
    }
}
