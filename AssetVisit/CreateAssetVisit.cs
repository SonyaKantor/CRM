using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetVisit
{
    public class CreateAssetVisit : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingService.Trace("BluTech.Asset.Plugins");
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            string visiterName = context.InputParameters.Contains("visitername") ? context.InputParameters["visitername"].ToString() : null;
            string visiterPhone = context.InputParameters.Contains("Phone") ? context.InputParameters["Phone"].ToString() : null;

            string email = context.InputParameters.Contains("email") ? context.InputParameters["email"].ToString() : null;
            DateTime visitTime = DateTime.Parse(context.InputParameters.Contains("VisitTime") ? context.InputParameters["VisitTime"].ToString() : null);
            EntityReference assetId = context.InputParameters.Contains("AssetId") ? context.InputParameters["AssetId"] as EntityReference : null;
            EntityReference contact = context.InputParameters.Contains("Contact") ? context.InputParameters["Contact"] as EntityReference : null;
            //       Money assetPrice = context.InputParameters.Contains("assetPrice") ? (Money)context.InputParameters["assetPrice"] : null;
            //       decimal assetRoomNumber = context.InputParameters.Contains("assetroomnumber") ? (decimal)context.InputParameters["assetroomnumber"] : 0;
            // ActionBL ac = new ActionBL(service, visiterName, visiterPhone, email, visitTime, assetId, contact, tracingService);
            //var actionBL = new ActionBL(service, "a", "555-0109", "a@gmail.com", DateTime.Now, new EntityReference("son_cities", new Guid("60110a45-7cd0-ed11-a7c7-000d3ab3f3a3")), new EntityReference("contact", new Guid("f84ab644-eac7-ed11-b597-000d3ab03356")));
            var actionBL = new ActionBL(service, tracingService);
            Guid visitId = actionBL.CreateAsset("a", "555-0109", "a@gmail.com", DateTime.Now, new EntityReference("son_cities", new Guid("60110a45-7cd0-ed11-a7c7-000d3ab3f3a3")));
            context.OutputParameters["AssetVisitId"] = new EntityReference("son_assetvisit", visitId);

        }

    }
}
