﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iikoLauncher.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("G:\\Мой Диск\\iikoLauncher\\iikoLauncher.xml")]
        public string ConnectionListPath {
            get {
                return ((string)(this["ConnectionListPath"]));
            }
            set {
                this["ConnectionListPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("admin")]
        public string IikoLogin {
            get {
                return ((string)(this["IikoLogin"]));
            }
            set {
                this["IikoLogin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("cmVzdG8jdGVzdA==")]
        public string IikoPassword {
            get {
                return ((string)(this["IikoPassword"]));
            }
            set {
                this["IikoPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("VGIjMTQ3ODUy")]
        public string AnyDeskPassword {
            get {
                return ((string)(this["AnyDeskPassword"]));
            }
            set {
                this["AnyDeskPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%ProgramFiles(x86)%\\AnyDesk\\AnyDesk.exe")]
        public string AnyDeskPath {
            get {
                return ((string)(this["AnyDeskPath"]));
            }
            set {
                this["AnyDeskPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%ProgramW6432%\\iiko\\iikoRMS")]
        public string IikoRMSPath {
            get {
                return ((string)(this["IikoRMSPath"]));
            }
            set {
                this["IikoRMSPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%ProgramW6432%\\iiko\\iikoChain")]
        public string IikoChainPath {
            get {
                return ((string)(this["IikoChainPath"]));
            }
            set {
                this["IikoChainPath"] = value;
            }
        }
    }
}
