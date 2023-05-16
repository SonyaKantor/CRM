using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using System.Collections;

namespace CustomWF
{
    public class TestCWF : CodeActivity
    {
        [Input("Owner")]
        [ReferenceTarget("contact")]
        public InArgument<EntityReference> contact { get; set; }

        [Output("asset amount")]
        public OutArgument<int> assetAmount { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            var _service = new OrganizationServiceContext(service);
            EntityReference contactRef = contact.Get(executionContext);
           // priceOut.Set(executionContext, inprice + 1000);
            assetAmount.Set(executionContext, getAmountOfAssets(service,contactRef.Id));

        }
        private int getAmountOfAssets(IOrganizationService service, Guid id)
        {
            QueryExpression q = new QueryExpression("son_cities");
            q.Criteria.AddCondition(new ConditionExpression("son_owner", ConditionOperator.Equal, id));
            q.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            q.ColumnSet = new ColumnSet("son_name");
            return service.RetrieveMultiple(q).Entities.Count;
        }
    }
}
