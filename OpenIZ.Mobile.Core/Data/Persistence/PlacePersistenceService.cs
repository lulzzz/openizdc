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
 * Date: 2016-6-28
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using OpenIZ.Mobile.Core.Data.Model;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persister which persists places
    /// </summary>
    public class PlacePersistenceService : EntityDerivedPersistenceService<Place, DbPlace>
    {
        /// <summary>
        /// Load to a model instance
        /// </summary>
        public override Place ToModelInstance(object dataInstance, SQLiteConnectionWithLock context)
        {
            var iddat = dataInstance as DbVersionedData;
            var place = dataInstance as DbPlace?? context.Table<DbPlace>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbe = dataInstance as DbEntity ?? context.Table<DbEntity>().Where(o => o.Uuid == place.Uuid).First();

            var retVal = m_entityPersister.ToModelInstance<Place>(dbe, context);
            retVal.IsMobile = place.IsMobile;
            retVal.Lat = place.Lat;
            retVal.Lng = place.Lng;
            retVal.LoadAssociations(context);

            return retVal;
        }

        /// <summary>
        /// Insert 
        /// </summary>
        public override Place Insert(SQLiteConnectionWithLock context, Place data)
        {
            var retVal = base.Insert(context, data);

            if (data.Services != null)
                base.UpdateAssociatedItems<PlaceService, Entity>(
                    new List<PlaceService>(),
                    data.Services,
                    data.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the place
        /// </summary>
        public override Place Update(SQLiteConnectionWithLock context, Place data)
        {
            var retVal = base.Update(context, data);

            byte[] sourceKey = data.Key.Value.ToByteArray();
            if (data.Services != null)
                base.UpdateAssociatedItems<PlaceService, Entity>(
                    context.Table<DbPlaceService>().Where(o => o.EntityUuid == sourceKey).ToList().Select(o => m_mapper.MapDomainInstance<DbPlaceService, PlaceService>(o)).ToList(),
                    data.Services,
                    data.Key,
                    context);

            return retVal;
        }
    }
}
