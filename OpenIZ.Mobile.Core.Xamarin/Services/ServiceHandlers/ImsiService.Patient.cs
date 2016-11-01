﻿using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Core.Model.Interfaces;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Extensions;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// IMSI Service
    /// </summary>
    public partial class ImsiService
    {

        /// <summary>
        /// Creates a patient.
        /// </summary>
        /// <param name="patientToInsert">The patient to be inserted.</param>
        /// <returns>Returns the inserted patient.</returns>
        [RestOperation(Method = "POST", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.WriteClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient CreatePatient([RestMessage(RestMessageFormat.SimpleJson)]Patient patientToInsert)
        {
            IPatientRepositoryService repository = ApplicationContext.Current.GetService<IPatientRepositoryService>();
            return repository.Insert(patientToInsert).GetLocked() as Patient;
        }

        /// <summary>
		/// Gets a patient.
		/// </summary>
		/// <returns>Returns the patient.</returns>
        [RestOperation(Method = "GET", UriPath = "/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.QueryClinicalData)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public IdentifiedData GetPatient()
        {
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);
            var patientService = ApplicationContext.Current.GetService<IPatientRepositoryService>();

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                MemoryCache.Current.RemoveObject(typeof(Patient), Guid.Parse(search["_id"].FirstOrDefault()));
                var patient = patientService.Get(Guid.Parse(search["_id"].FirstOrDefault()), Guid.Empty);
                patient = patient.LoadDisplayProperties().LoadImmediateRelations();
                return patient;
            }
            else
            {


                int totalResults = 0,
                    offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                    count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                IEnumerable<Patient> retVal = null;

                // Any filter
                if (search.ContainsKey("any") || search.ContainsKey("any[]"))
                {

                    this.m_tracer.TraceVerbose("Freetext search: {0}", MiniImsServer.CurrentContext.Request.Url.Query);


                    var values = search.ContainsKey("any") ? search["any"] : search["any[]"];
                    // Filtes
                    var fts = ApplicationContext.Current.GetService<IFreetextSearchService>();
                    retVal = fts.Search<Patient>(values.ToArray(), offset, count, out totalResults);
                    search.Remove("any");
                    search.Remove("any[]");
                }

                if (search.Keys.Count(o => !o.StartsWith("_")) > 0)
                {
                    var predicate = QueryExpressionParser.BuildLinqExpression<Patient>(search);
                    this.m_tracer.TraceVerbose("Searching Patients : {0} / {1}", MiniImsServer.CurrentContext.Request.Url.Query, predicate);

                    var tret = patientService.Find(predicate, offset, count, out totalResults);
                    if (retVal == null)
                        retVal = tret;
                    else
                        retVal = retVal.OfType<IIdentifiedEntity>().Intersect(tret.OfType<IIdentifiedEntity>(), new KeyComparer()).OfType<Patient>();
                }

                // Serialize the response
                var itms = retVal.OfType<Patient>().Select(o => o.LoadDisplayProperties().LoadImmediateRelations());
                return new Bundle()
                {
                    Item = itms.OfType<IdentifiedData>().ToList(),
                    Offset = offset,
                    Count = itms.Count(),
                    TotalResults = totalResults
                };
            }
        }

        /// <summary>
        /// Get a patient
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/Empty/Patient", FaultProvider = nameof(ImsiFault))]
        [Demand(PolicyIdentifiers.Login)]
        [return: RestMessage(RestMessageFormat.SimpleJson)]
        public Patient GetEmptyPatient()
        {
            // TODO: Select a default identifier domain 
            // Return the patient
            var retVal = new Patient()
            {
                Key = Guid.NewGuid(),
                Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        },
                Addresses = new List<EntityAddress>()
                {
                    new EntityAddress() {
                        AddressUseKey = AddressUseKeys.HomeAddress,
                        Component = new List<EntityAddressComponent>() {
                            new EntityAddressComponent(AddressComponentKeys.CensusTract, "373a1702-72d8-40a8-b0f5-0e1fd7d86d97")
                        }
                    }
                },
                DateOfBirth = DateTime.Now,
                Identifiers = new List<EntityIdentifier>()
                {
                    new EntityIdentifier(null, null)
                    {
                        Authority = new AssigningAuthority() { DomainName = "NEW" },
                        Value = ""
                    }
                },
                Relationships = new List<EntityRelationship>()
                {
                    new EntityRelationship(EntityRelationshipTypeKeys.Mother, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        },
                        Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        }
                    }),
                    new EntityRelationship(null, new Person() {
                        Telecoms = new List<EntityTelecomAddress>()
                        {
                            new EntityTelecomAddress(TelecomAddressUseKeys.MobileContact, null)
                        },
                        Names = new List<EntityName>()
                        {
                            new EntityName() { NameUseKey = NameUseKeys.OfficialRecord }
                        }
                    }),
                    new EntityRelationship(EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation, Guid.Parse("d0f4a878-13cb-4509-9b4f-80b2c1548d2b")) // TODO: Get user's current location
                }
            };
            retVal.SetDelayLoad(true);
            // HACK: For form which is expecting $0ther
            (retVal.Relationships[1].TargetEntity as Entity).Telecoms[0].SetDelayLoad(false);
            (retVal.Relationships[0].TargetEntity as Entity).Telecoms[0].SetDelayLoad(false);
            // Serialize the response
            return retVal;
        }
    }
}
