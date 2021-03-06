﻿/*
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Menu information
    /// </summary>
    [JsonObject]
    public class MenuInformation
    {


        /// <summary>
        /// Get or sets the menu
        /// </summary>
        [JsonProperty("menu")]
        public List<MenuInformation> Menu { get; set; }

        /// <summary>
        /// Icon text
        /// </summary>
        [JsonProperty("icon")]
        public String Icon { get; set; }

        /// <summary>
        /// Text for the menu item
        /// </summary>
        [JsonProperty("text")]
        public String Text { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        [JsonProperty("action")]
        public String Action { get; set; }
    }
}
