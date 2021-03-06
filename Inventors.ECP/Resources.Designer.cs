﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventors.ECP {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Inventors.ECP.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Too few bytes in beacon packet.
        /// </summary>
        internal static string BEACON_PACKET_DATA_ERROR {
            get {
                return ResourceManager.GetString("BEACON_PACKET_DATA_ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection is null.
        /// </summary>
        internal static string CONNECTION_NULL {
            get {
                return ResourceManager.GetString("CONNECTION_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device is null.
        /// </summary>
        internal static string DEVICE_NULL {
            get {
                return ResourceManager.GetString("DEVICE_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Frame is null.
        /// </summary>
        internal static string FRAME_IS_NULL {
            get {
                return ResourceManager.GetString("FRAME_IS_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The frame is less than 2 bytes long.
        /// </summary>
        internal static string INVALID_FRAME_TOO_SHORT {
            get {
                return ResourceManager.GetString("INVALID_FRAME_TOO_SHORT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid function code.
        /// </summary>
        internal static string INVALID_FUNCTION_CODE {
            get {
                return ResourceManager.GetString("INVALID_FUNCTION_CODE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid IP Address.
        /// </summary>
        internal static string INVALID_IP_ADDRESS {
            get {
                return ResourceManager.GetString("INVALID_IP_ADDRESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid operation while the port is open.
        /// </summary>
        internal static string INVALID_OPERATION_WHILE_OPEN {
            get {
                return ResourceManager.GetString("INVALID_OPERATION_WHILE_OPEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected length, expected [ {0} ] but it was [ {1} ].
        /// </summary>
        internal static string INVALID_PACKET_LENGTH {
            get {
                return ResourceManager.GetString("INVALID_PACKET_LENGTH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid packet type.
        /// </summary>
        internal static string INVALID_PACKET_TYPE {
            get {
                return ResourceManager.GetString("INVALID_PACKET_TYPE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid response content.
        /// </summary>
        internal static string INVALID_RESPONSE_CONTENT {
            get {
                return ResourceManager.GetString("INVALID_RESPONSE_CONTENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid argument, time is null.
        /// </summary>
        internal static string INVALID_TIME_NULL {
            get {
                return ResourceManager.GetString("INVALID_TIME_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Passed null values for layer or deviceData.
        /// </summary>
        internal static string LAYER_OR_DEVICE_DATA_IS_NULL {
            get {
                return ResourceManager.GetString("LAYER_OR_DEVICE_DATA_IS_NULL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No response from the slave.
        /// </summary>
        internal static string NO_RESPONSE {
            get {
                return ResourceManager.GetString("NO_RESPONSE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot change the port while the client is open.
        /// </summary>
        internal static string PORT_CHANGE_WHILE_OPEN {
            get {
                return ResourceManager.GetString("PORT_CHANGE_WHILE_OPEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown type of length encoding:.
        /// </summary>
        internal static string UNKNOWN_PACKET_ENCODING {
            get {
                return ResourceManager.GetString("UNKNOWN_PACKET_ENCODING", resourceCulture);
            }
        }
    }
}
