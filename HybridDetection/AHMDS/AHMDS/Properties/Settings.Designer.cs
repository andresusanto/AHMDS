﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AHMDS.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\DB\\ahmds.accdb;Pers" +
            "ist Security Info=True")]
        public string ahmdsConnectionString {
            get {
                return ((string)(this["ahmdsConnectionString"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Perl\\perl\\bin\\perl.exe")]
        public string PerlLocation {
            get {
                return ((string)(this["PerlLocation"]));
            }
            set {
                this["PerlLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Sandbox\\AHMDS\\")]
        public string SandboxieBoxLocation {
            get {
                return ((string)(this["SandboxieBoxLocation"]));
            }
            set {
                this["SandboxieBoxLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Sandboxie\\32\\SbieDll.dll")]
        public string SandboxieDllLocation {
            get {
                return ((string)(this["SandboxieDllLocation"]));
            }
            set {
                this["SandboxieDllLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Sandboxie\\Start.exe")]
        public string SandboxieExeLocation {
            get {
                return ((string)(this["SandboxieExeLocation"]));
            }
            set {
                this["SandboxieExeLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Malware1</string>
  <string>Malware2</string>
  <string>Malware3</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection SandboxNames {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["SandboxNames"]));
            }
            set {
                this["SandboxNames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files (x86)\\swipl")]
        public string SWIPath {
            get {
                return ((string)(this["SWIPath"]));
            }
            set {
                this["SWIPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("400")]
        public int MalwareScoreThreshold {
            get {
                return ((int)(this["MalwareScoreThreshold"]));
            }
            set {
                this["MalwareScoreThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("350")]
        public int APICallScoreThreshold {
            get {
                return ((int)(this["APICallScoreThreshold"]));
            }
            set {
                this["APICallScoreThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int DynamicAnalysisDuration {
            get {
                return ((int)(this["DynamicAnalysisDuration"]));
            }
            set {
                this["DynamicAnalysisDuration"] = value;
            }
        }
    }
}
