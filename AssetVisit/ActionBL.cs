using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace AssetVisit
{
    public class ActionBL
    {
        private readonly IOrganizationService service;
        private readonly ITracingService tracingService;

        public ActionBL(IOrganizationService service, ITracingService tracingService = null)
        {
            this.service = service;
            this.tracingService = tracingService;
            
        }
        public Guid GetAccountByPhone(IOrganizationService service, string visiterPhone, string email)
        {
            QueryExpression q = new QueryExpression("contact");
            q.Criteria = new FilterExpression();
            FilterExpression filter = q.Criteria.AddFilter(LogicalOperator.Or);
            filter.AddCondition(new ConditionExpression("telephone1", ConditionOperator.Equal, visiterPhone));
            filter.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            EntityCollection ec = service.RetrieveMultiple(q);
            if (ec != null && ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            else
            {
                return Guid.Empty;
            }
        }
        public Guid GetLeadByPhone(IOrganizationService service, string visiterPhone, string email)
        {
            QueryExpression q = new QueryExpression("lead");
            FilterExpression filter = q.Criteria.AddFilter(LogicalOperator.Or);
            
            filter.AddCondition(new ConditionExpression("telephone1", ConditionOperator.Equal, visiterPhone));

            filter.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            EntityCollection ec = service.RetrieveMultiple(q);
            if (ec != null && ec.Entities.Count > 0)
            {
                return ec.Entities.FirstOrDefault().Id;
            }
            else
            {
                return Guid.Empty;
            }

        }
        public Guid CreateAsset(string visiterName, string visiterPhone, string email, DateTime visitTime, EntityReference assetId, EntityReference contact = null)
        {
            Entity visitToCreate = new Entity("son_assetvisit");
            Guid accId;
            if (contact != null)
            {
                accId = contact.Id;
                visitToCreate.Attributes.Add("regardingobjectid", new EntityReference("contact", accId));
            }

            else
            {
                accId = GetAccountByPhone(service, visiterPhone,email);

                if (accId == Guid.Empty)
                {
                    Guid leadId = GetLeadByPhone(service,visiterPhone,email);
                    if (leadId == Guid.Empty)
                    {
                        Entity leadToCreate = new Entity("lead");
                        leadToCreate.Attributes.Add("firstname", visiterName);
                        leadToCreate.Attributes.Add("telephone1", visiterPhone);
                        leadToCreate.Attributes.Add("emailaddress1", email);
                        leadId = service.Create(leadToCreate);

                    }
                    visitToCreate.Attributes.Add("regardingobjectid", new EntityReference("lead", leadId));
                }
                else
                    visitToCreate.Attributes.Add("regardingobjectid", new EntityReference("contact", accId));
            }


            visitToCreate.Attributes.Add("subject", visiterName);
            visitToCreate.Attributes.Add("son_visittime", visitTime);
            visitToCreate.Attributes.Add("son_asset", new EntityReference("son_cities", assetId.Id));
            Guid visitId = service.Create(visitToCreate);
            return visitId;
        }
    }
}
