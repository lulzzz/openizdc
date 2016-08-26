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
 * Date: 2016-7-2
 */
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Persistence service for matrials
    /// </summary>
    public class MaterialPersistenceService : EntityDerivedPersistenceService<Material, DbMaterial>
    {

        /// <summary>
        /// Convert persistence model to business objects
        /// </summary>
        public override Material ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            return this.ToModelInstance<Material>(dataInstance, context, loadFast);
        }

        /// <summary>
        /// Creates the specified model instance
        /// </summary>
        internal TModel ToModelInstance<TModel>(object rawInstance, SQLiteConnectionWithLock context, bool loadFast)
            where TModel : Material, new()
        {
            var iddat = rawInstance as DbIdentified;
            var dataInstance = rawInstance as DbMaterial ?? context.Table<DbMaterial>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbe = rawInstance as DbEntity ?? context.Table<DbEntity>().Where(o => o.Uuid == dataInstance.Uuid).First();
            var retVal = this.m_entityPersister.ToModelInstance<TModel>(dbe, context, loadFast);
            retVal.ExpiryDate = dataInstance.ExpiryDate;
            retVal.IsAdministrative = dataInstance.IsAdministrative;
            retVal.Quantity = dataInstance.Quantity;
            if(dataInstance.QuantityConceptUuid != null)
                retVal.QuantityConceptKey = new Guid(dataInstance.QuantityConceptUuid);
            if(dataInstance.FormConceptUuid != null)
                retVal.FormConceptKey = new Guid(dataInstance.FormConceptUuid);
            retVal.LoadAssociations(context);
            return retVal;

        }

        /// <summary>
        /// Insert the material
        /// </summary>
        public override Material Insert(SQLiteConnectionWithLock context, Material data)
        {
            data.FormConcept?.EnsureExists(context);
            data.QuantityConcept?.EnsureExists(context);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified material
        /// </summary>
        public override Material Update(SQLiteConnectionWithLock context, Material data)
        {
            data.FormConcept?.EnsureExists(context);
            data.QuantityConcept?.EnsureExists(context);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Update(context, data);
        }
    }
}
