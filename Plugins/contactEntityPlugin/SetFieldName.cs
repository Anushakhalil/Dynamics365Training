using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contactEntityPlugin
{
    public class SetFieldName : IPlugin
    {
        ITracingService tracingService;
        IOrganizationService service;
        IPluginExecutionContext context;
        Entity entity;

        public void Execute(IServiceProvider serviceProvider)
        {
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                entity = (Entity)context.InputParameters["Target"];

                try
                {
                    if (entity.LogicalName == "contact")
                    {
                        switch (context.Stage)
                        {
                            case 10://Pre validation

                                switch (context.MessageName)
                                {
                                    case "Create":
                                        break;

                                    case "Update":
                                        break;

                                    default:
                                        break;
                                }
                                break;

                            case 20://Pre-operation

                                switch (context.MessageName)
                                {
                                    case "Create":

                                        break;

                                    case "Update":
                                        break;

                                    default:
                                        break;
                                }
                                break;

                            case 40://post-operation

                                switch (context.MessageName)
                                {
                                    case "Create":
                                        break;

                                    case "Update":
                                        break;

                                    default:
                                        break;
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in Books Plugin.", ex);
                }
            }
        }

        private void SetField()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in function.", ex);
            }
        }
    }
}
