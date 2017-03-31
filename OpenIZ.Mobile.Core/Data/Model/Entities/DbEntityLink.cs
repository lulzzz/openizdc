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
 * Date: 2017-2-4
 */
using System;
using SQLite.Net;
using SQLite.Net.Attributes;
using OpenIZ.Core.Data.QueryBuilder.Attributes;

namespace OpenIZ.Mobile.Core.Data.Model.Entities
{
	/// <summary>
	/// Represents a class which is linked to an entity
	/// </summary>
	public abstract class DbEntityLink : DbIdentified
	{

		/// <summary>
		/// Gets or sets the entity identifier.
		/// </summary>
		/// <value>The entity identifier.</value>
		[Column("entity_uuid"), Indexed, MaxLength(16), NotNull, ForeignKey(typeof(DbEntity), nameof(DbEntity.Uuid))]
		public byte[] SourceUuid {
			get;
			set;
		}



    }
}

