using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckIntegerPlugin
{
    public class CheckIntegerPlugin: IPlugin
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
                if (entity.LogicalName == "ankh_book")
                {
                    switch (context.Stage)
                    {
                        case 10://Pre validation

                            switch (context.MessageName)
                            {
                                case "Create":
                                    break;

                                case "Update":
                                    tracingService.Trace("entered case create");
                                    CheckForUpdate();
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case 20://Pre-operation

                            switch (context.MessageName)
                            {
                                case "Create":
                                    tracingService.Trace("entered case create");
                                    CheckForInteger();
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
                throw new InvalidPluginExecutionException(ex.Message);
                }
        }
    }

    private void CheckForInteger()
    {
        try
        {
            tracingService.Trace("entered func try");
            if (entity.GetAttributeValue<Int32>("ankh_updated_price") < entity.GetAttributeValue<Int32>("ankh_previous_amount"))
            {
                throw new InvalidPluginExecutionException("Updated price cannot be lesser than previoud price");
            }
        }

        catch (InvalidPluginExecutionException ex)
        {
            throw new InvalidPluginExecutionException(ex.Message);
        }
    }

        private void CheckForUpdate()
        {
            try
            {
                tracingService.Trace("entered try");

                int IV_preprice;
                int IV_updatedprice;

                int preamount = entity.GetAttributeValue<Int32>("ankh_previous_amount");
                int updatedprice = entity.GetAttributeValue<Int32>("ankh_updated_price");

                if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity)
                {
                    tracingService.Trace("entered frist if");
                    Entity PreImage = context.PreEntityImages["PreImage"];

                    if (PreImage.Attributes.Contains("ankh_previous_amount") && PreImage.Attributes.Contains("ankh_updated_price"))
                    {
                        tracingService.Trace("entered second if");
                        IV_preprice = PreImage.GetAttributeValue<Int32>("ankh_previous_amount");
                        IV_updatedprice = PreImage.GetAttributeValue<Int32>("ankh_updated_price");

                        if (preamount != 0 && updatedprice == 0)
                        {
                            tracingService.Trace("first case");
                            if (IV_updatedprice < preamount)
                            {
                                tracingService.Trace("frist case if");
                                throw new InvalidPluginExecutionException("Previous price must be less than updated");
                            }
                        }
                        else if (preamount == 0 && updatedprice != 0)
                        {
                            tracingService.Trace("second case");
                            if (updatedprice < IV_preprice)
                            {
                                tracingService.Trace("second case if ");
                                throw new InvalidPluginExecutionException("Updated price cannot be lesser than previoud price");
                            }
                        }
                        else if (preamount != 0 && updatedprice != 0)
                        {
                            tracingService.Trace("third case");
                            if (updatedprice < preamount)
                            {
                                tracingService.Trace("third case if");
                                throw new InvalidPluginExecutionException("Updated price cannot be lesser than previoud price");
                            }
                        }
                    }
                }
            }

            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}