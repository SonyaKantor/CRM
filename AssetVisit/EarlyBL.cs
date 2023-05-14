using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using Nipendo.Common.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crmBL
{
    public class EarlyBL
    {
        private readonly IOrganizationService service;
        private readonly ITracingService tracingService;

        public EarlyBL(IOrganizationService service, ITracingService tracingService = null)
        {
            this.service = service;
            this.tracingService = tracingService;

        }
        public Guid GetAccountByPhone(IOrganizationService service, string visiterPhone, string email)
        {
            QueryExpression q = new QueryExpression(Contact.EntityLogicalName);
            q.Criteria = new FilterExpression();
            FilterExpression filter = q.Criteria.AddFilter(LogicalOperator.Or);
            filter.AddCondition(new ConditionExpression(Contact.Fields.Telephone1, ConditionOperator.Equal, visiterPhone));
            filter.AddCondition(new ConditionExpression(Contact.Fields.EMailAddress1, ConditionOperator.Equal, email));
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
            QueryExpression q = new QueryExpression(Lead.EntityLogicalName);
            FilterExpression filter = q.Criteria.AddFilter(LogicalOperator.Or);

            filter.AddCondition(new ConditionExpression(Lead.Fields.Telephone1, ConditionOperator.Equal, visiterPhone));

            filter.AddCondition(new ConditionExpression(Lead.Fields.EMailAddress1, ConditionOperator.Equal, email));
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
            son_Assetvisit visitToCreate = new son_Assetvisit();
            
            Guid accId;
            if (contact != null)
            {
                accId = contact.Id;
                visitToCreate.RegardingObjectId = new EntityReference(Contact.EntityLogicalName, accId);
                
            }

            else
            {
                accId = GetAccountByPhone(service, visiterPhone, email);

                if (accId == Guid.Empty)
                {
                    Guid leadId = GetLeadByPhone(service, visiterPhone, email);
                    if (leadId == Guid.Empty)
                    {
                        Lead leadToCreate = new Lead();
                        leadToCreate.FirstName = visiterName;
                        leadToCreate.Telephone1 = visiterPhone;
                        leadToCreate.EMailAddress1= email;
                        leadId = service.Create(leadToCreate);

                    }
                    visitToCreate.RegardingObjectId = new EntityReference(Lead.EntityLogicalName, leadId);
                }
                else
                    visitToCreate.RegardingObjectId = new EntityReference(Contact.EntityLogicalName, accId);
            }


            visitToCreate.Subject= visiterName;
            visitToCreate.son_Visittime = visitTime;
            visitToCreate.son_Asset = new EntityReference(son_Cities.EntityLogicalName, assetId.Id);
            Guid visitId = service.Create(visitToCreate);
            return visitId;
        }
    }
}
