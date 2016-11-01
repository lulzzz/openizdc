﻿using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Extensions;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// IMSI Service handler for acts
    /// </summary>
    public partial class ImsiService
    {

        /// <summary>
        /// Creates an act.
        /// </summary>
        /// <param name="actToInsert">The act to be inserted.</param>
        /// <returns>Returns the inserted act.</returns>
        [RestOperation(Method = "POST", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act CreateAct([RestMessage(RestMessageFormat.SimpleJson)]Act actToInsert)
        {
            IActRepositoryService actService = ApplicationContext.Current.GetService<IActRepositoryService>();
            return actService.Insert(actToInsert);
        }

        /// <summary>
        /// Gets a list of acts.
        /// </summary>
        /// <returns>Returns a list of acts.</returns>
        [RestOperation(Method = "GET", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetAct()
        {
            var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Act), Guid.Parse(search["_id"].FirstOrDefault()));
                var act = actRepositoryService.Get<Act>(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                act = act.LoadDisplayProperties();
                return act;
            }
            else
            {
                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                var results = actRepositoryService.Find(QueryExpressionParser.BuildLinqExpression<Act>(search, null, false), offset, count, out totalResults);

                results = results.Select(a => a.LoadDisplayProperties()); //.LoadImmediateRelations());
                results.Select(a => a).ToList().ForEach(a => a.Relationships.OrderBy(r => r.TargetAct.CreationTime));

                // 
                return new Bundle
                {
                    Count = results.Count(),
                    Item = results.OfType<IdentifiedData>().ToList(),
                    Offset = 0,
                    TotalResults = totalResults
                };
            }
        }

        /// <summary>
        /// Deletes the act
        /// </summary>
        [RestOperation(Method = "DELETE", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.DeleteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act DeleteAct()
        {
            var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();

            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                var keyid = Guid.Parse(search["_id"].FirstOrDefault());
                MemoryCache.Current.RemoveObject(typeof(Act), keyid);
                var act = actRepositoryService.Get<Act>(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                if (act == null) throw new KeyNotFoundException();

                return actRepositoryService.Obsolete<Act>(keyid);
            }
            else
                throw new ArgumentNullException("_id");
        }


        /// <summary>
        /// Updates an act.
        /// </summary>
        /// <param name="act">The act to update.</param>
        /// <returns>Returns the updated act.</returns>
        [RestOperation(Method = "PUT", UriPath = "/Act", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Act UpdateAct([RestMessage(RestMessageFormat.SimpleJson)] Act act)
        {
            var query = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            Guid actKey = Guid.Empty;
            Guid actVersionKey = Guid.Empty;

            if (query.ContainsKey("_id") && Guid.TryParse(query["_id"][0], out actKey) && query.ContainsKey("_versionId") && Guid.TryParse(query["_versionId"][0], out actVersionKey))
            {
                if (act.Key == actKey && act.VersionKey == actVersionKey)
                {
                    var actRepositoryService = ApplicationContext.Current.GetService<IActRepositoryService>();

                    return actRepositoryService.Save(act);
                }
                else
                {
                    throw new ArgumentException("Act not found");
                }
            }
            else
            {
                throw new ArgumentException("Act not found");
            }
        }
    }
}
