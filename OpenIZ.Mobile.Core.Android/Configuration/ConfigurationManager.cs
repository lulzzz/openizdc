﻿/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-6-14
 */
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using OpenIZ.Mobile.Core.Configuration;
using Android.Content.Res;
using OpenIZ.Mobile.Core.Xamarin.Http;
using System.Collections.Generic;
using OpenIZ.Mobile.Core.Diagnostics;
using System.Security.Cryptography;
using OpenIZ.Mobile.Core.Configuration.Data;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services.Impl;
using OpenIZ.Mobile.Core.Xamarin.Security;
using OpenIZ.Mobile.Core.Data;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using AndroidOS = Android.OS;
using OpenIZ.Mobile.Core.Xamarin.Services;
using OpenIZ.Mobile.Core.Xamarin.Threading;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Core.Services.Impl;
using OpenIZ.Core.Protocol;
using OpenIZ.Mobile.Core.Android.Net;
using OpenIZ.Mobile.Core.Search;
using OpenIZ.Mobile.Core.Protocol;
using OpenIZ.Mobile.Core.Xamarin.Configuration;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;
using OpenIZ.Mobile.Core.Android.Diagnostics;
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Mobile.Core.Xamarin.Rules;
using OpenIZ.Mobile.Core.Xamarin.Warehouse;

namespace OpenIZ.Mobile.Core.Android.Configuration
{
    /// <summary>
    /// Configuration manager for the application
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {

        private const int PROVIDER_RSA_FULL = 1;

        // Tracer
        private Tracer m_tracer;

        // Configuration
        private OpenIZConfiguration m_configuration;

        // Configuration path
        private readonly String m_configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZ.config");

        /// <summary>
        /// Returns true if OpenIZ is configured
        /// </summary>
        /// <value><c>true</c> if this instance is configured; otherwise, <c>false</c>.</value>
        public bool IsConfigured
        {
            get
            {
                return File.Exists(this.m_configPath);
            }
        }

        /// <summary>
        /// Get a bare bones configuration
        /// </summary>
        public static OpenIZConfiguration GetDefaultConfiguration()
        {
            // TODO: Bring up initial settings dialog and utility
            var retVal = new OpenIZConfiguration();

            // Inital data source
            DataConfigurationSection dataSection = new DataConfigurationSection()
            {
                MainDataSourceConnectionStringName = "openIzData",
                MessageQueueConnectionStringName = "openIzQueue",
                ConnectionString = new System.Collections.Generic.List<ConnectionString>() {
                    new ConnectionString () {
                        Name = "openIzData",
                        Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "OpenIZ.sqlite")
                    },
                    new ConnectionString () {
                        Name = "openIzSearch",
                        Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "OpenIZ.ftsearch.sqlite")
                    },
                    new ConnectionString () {
                        Name = "openIzQueue",
                        Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "MessageQueue.sqlite")
                    },
                    new ConnectionString () {
                        Name = "openIzWarehouse",
                        Value = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "OpenIZ.warehouse.sqlite")
                    }
                }
            };

            // Initial Applet configuration
            AppletConfigurationSection appletSection = new AppletConfigurationSection()
            {
                AppletDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "applets"),
                AppletGroupOrder = new System.Collections.Generic.List<string>() {
                    "Patient Management",
                    "Encounter Management",
                    "Stock Management",
                    "Administration"
                },
                StartupAsset = "org.openiz.core",
                AuthenticationAsset = "/tz/timr/applet/views/security/login.html"
            };

            // Protocol 
            ForecastingConfigurationSection forecastingSection = new ForecastingConfigurationSection()
            {
                ProtocolSourceDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "protocols"),
            };

            // Initial applet style
            ApplicationConfigurationSection appSection = new ApplicationConfigurationSection()
            {
                Style = StyleSchemeType.Dark,
                UserPrefDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "userpref"),
                ServiceTypes = new List<string>() {
                    
                    typeof(LocalPolicyDecisionService).AssemblyQualifiedName,
                    typeof(LocalPolicyInformationService).AssemblyQualifiedName,
                    typeof(LocalPatientService).AssemblyQualifiedName,
                    typeof(LocalPlaceService).AssemblyQualifiedName,
                    //typeof(LocalAlertService).AssemblyQualifiedName,
                    typeof(LocalConceptService).AssemblyQualifiedName,
                    typeof(LocalEntityRepositoryService).AssemblyQualifiedName,
                    typeof(LocalOrganizationService).AssemblyQualifiedName,
                    typeof(LocalRoleProviderService).AssemblyQualifiedName,
                    typeof(LocalSecurityService).AssemblyQualifiedName,
                    typeof(LocalMaterialService).AssemblyQualifiedName,
                    typeof(LocalBatchService).AssemblyQualifiedName,
                    typeof(BusinessRulesDaemonService).AssemblyQualifiedName,
                    typeof(SQLiteDatawarehouse).AssemblyQualifiedName,
                    typeof(LocalActService).AssemblyQualifiedName,
                    typeof(LocalProviderService).AssemblyQualifiedName,
                    typeof(AndroidNetworkInformationService).AssemblyQualifiedName,
                    typeof(MemoryQueryPersistenceService).AssemblyQualifiedName,
                    typeof(CarePlanManagerService).AssemblyQualifiedName,
                    typeof(LocalEntitySource).AssemblyQualifiedName,
                    typeof(MiniImsServer).AssemblyQualifiedName,
                    typeof(MemoryCacheService).AssemblyQualifiedName,
                    typeof(MemorySessionManagerService).AssemblyQualifiedName,
                    typeof(OpenIZThreadPool).AssemblyQualifiedName,
                    typeof(SimplePatchService).AssemblyQualifiedName,
                    typeof(SimpleCarePlanService).AssemblyQualifiedName,
                    typeof(SimpleClinicalProtocolRepositoryService).AssemblyQualifiedName,
                    typeof(SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid).AssemblyQualifiedName,
                    typeof(SearchIndexService).AssemblyQualifiedName,
                },
                Cache = new CacheConfiguration()
                {
                    MaxAge = new TimeSpan(0, 5, 0).Ticks,
                    MaxSize = 1000,
                    MaxDirtyAge = new TimeSpan(0, 20, 0).Ticks,
                    MaxPressureAge = new TimeSpan(0, 2, 0).Ticks
                }
            };

            // Security configuration
            var wlan = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(o => o.NetworkInterfaceType == NetworkInterfaceType.Ethernet && o.Description.StartsWith("wlan"));
            String macAddress = Guid.NewGuid().ToString();
            if (wlan != null)
                macAddress = wlan.GetPhysicalAddress().ToString();
            //else 

            SecurityConfigurationSection secSection = new SecurityConfigurationSection()
            {
                DeviceName = String.Format("{0}-{1}", AndroidOS.Build.Model, macAddress).Replace(" ", "")
            };

            // Device key
            var certificate = X509CertificateUtils.FindCertificate(X509FindType.FindBySubjectName, StoreLocation.LocalMachine, StoreName.My, String.Format("DN={0}.mobile.openiz.org", macAddress));
            secSection.DeviceSecret = certificate?.Thumbprint;

            // Rest Client Configuration
            ServiceClientConfigurationSection serviceSection = new ServiceClientConfigurationSection()
            {
                RestClientType = typeof(RestClient)
            };

            // Trace writer
#if DEBUG
			DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection () {
				TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration> () {
					new TraceWriterConfiguration () { 
						Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
						InitializationData = "OpenIZ",
						TraceWriter = new LogTraceWriter (System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
					},
					new TraceWriterConfiguration() {
						Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
						InitializationData = "OpenIZ",
						TraceWriter = new FileTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
					},
                    new TraceWriterConfiguration() {
                        Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
                        InitializationData = "OpenIZ",
                        TraceWriter = new AndroidLogTraceWriter(System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
                    }
                }
			};
#else
            DiagnosticsConfigurationSection diagSection = new DiagnosticsConfigurationSection()
            {
                TraceWriter = new List<TraceWriterConfiguration>() {
                    new TraceWriterConfiguration () {
                        Filter = System.Diagnostics.Tracing.EventLevel.Warning,
                        InitializationData = "OpenIZ",
                        TraceWriter = new FileTraceWriter (System.Diagnostics.Tracing.EventLevel.Warning, "OpenIZ")
                    }
                }
            };
#endif
            retVal.Sections.Add(appletSection);
            retVal.Sections.Add(dataSection);
            retVal.Sections.Add(diagSection);
            retVal.Sections.Add(appSection);
            retVal.Sections.Add(secSection);
            retVal.Sections.Add(serviceSection);
            retVal.Sections.Add(forecastingSection);
            retVal.Sections.Add(new SynchronizationConfigurationSection()
            {
                PollInterval = new TimeSpan(0, 5, 0)
            });
            return retVal;
        }


        /// <summary>
        /// Creates a new instance of the configuration manager with the specified configuration file
        /// </summary>
        public ConfigurationManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.Android.Configuration.ConfigurationManager"/> class.
        /// </summary>
        /// <param name="config">Config.</param>
        public ConfigurationManager(OpenIZConfiguration config)
        {
            this.m_configuration = config;
        }

        /// <summary>
        /// Load the configuration
        /// </summary>
        public void Load()
        {
            // Configuration exists?
            if (this.IsConfigured)
                using (var fs = File.OpenRead(this.m_configPath))
                {
                    this.m_configuration = OpenIZConfiguration.Load(fs);
                }


        }


        /// <summary>
        /// Save the configuration to the default location
        /// </summary>
        public void Save()
        {
            this.Save(this.m_configuration);
        }
        /// <summary>
        /// Save the specified configuration
        /// </summary>
        /// <param name="config">Config.</param>
        public void Save(OpenIZConfiguration config)
        {
            try
            {
                this.m_tracer?.TraceInfo("Saving configuration to {0}...", this.m_configPath);
                if (!Directory.Exists(Path.GetDirectoryName(this.m_configPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(this.m_configPath));

                using (FileStream fs = File.Create(this.m_configPath))
                {
                    config.Save(fs);
                    fs.Flush();
                }
            }
            catch (Exception e)
            {
                this.m_tracer?.TraceError(e.ToString());
            }
        }

        /// <summary>
        /// Get the configuration
        /// </summary>
        public OpenIZConfiguration Configuration
        {
            get
            {
                return this.m_configuration;
            }
        }

    }
}

