/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Xamarin.Net;
using Android.Net;
using OpenIZ.Mobile.Core.Services;
using System.Net.NetworkInformation;
using AndroidOS = Android.OS;

namespace OpenIZ.Mobile.Core.Android.Net
{
    /// <summary>
    /// Android specific network information service
    /// </summary>
    /// TODO: Add configuration for roaming
    public class AndroidNetworkInformationService : NetworkInformationService
    {

        private ConnectivityManager m_connManager = null;

        public AndroidNetworkInformationService()
        {
            this.m_connManager = (ConnectivityManager)((Xamarin.XamarinApplicationContext.Current as AndroidApplicationContext).Context).GetSystemService(Context.ConnectivityService);
        }
        /// <summary>
        /// Get interfaces
        /// </summary>
        public override IEnumerable<NetworkInterfaceInfo> GetInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces().Select(o => new NetworkInterfaceInfo(
                o.Name, o.GetPhysicalAddress().ToString(), o.OperationalStatus == OperationalStatus.Up, AndroidOS.Build.Model, o.GetIPProperties().UnicastAddresses.FirstOrDefault().ToString()
            ));
        }

        /// <summary>
        /// Is network available
        /// </summary>
        public override bool IsNetworkAvailable
        {
            get
            {
                
                return this.m_connManager.ActiveNetworkInfo != null && this.m_connManager.ActiveNetworkInfo.IsConnected &&
                    !this.m_connManager.ActiveNetworkInfo.IsRoaming;
            }
        }

        /// <summary>
        /// True if the network is wifi
        /// </summary>
        public override bool IsNetworkWifi
        {
            get
            {
                return this.IsNetworkAvailable && this.m_connManager.ActiveNetworkInfo?.Type == ConnectivityType.Wifi;
            }
        }
    }
}