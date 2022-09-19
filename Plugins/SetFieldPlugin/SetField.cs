using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetFieldPlugin
{
    public class SetField : IPlugin
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
                    if (entity.LogicalName == "ankh_contact")
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
                                        SetFieldMethod();
                                        break;

                                    case "Update":
                                        SetFieldMethod();
                                        break;

                                    default:
                                        break;
                                }
                                break;

                            case 40://post-operation

                                switch (context.MessageName)
                                {
                                    case "Create":
                                        CopyFields();
                                        break;

                                    case "Update":
                                        UpdateFieldTracingMethod();
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

        private void SetFieldMethod()
        {
            try
            {
                if (entity.Attributes.Contains("ankh_book"))
                {       
                    Guid bookEntityId = entity.GetAttributeValue<EntityReference>("ankh_book").Id;
                    Entity bookEntity = service.Retrieve("ankh_book", bookEntityId, new ColumnSet("ankh_title"));
                    entity.Attributes["ankh_booktitle"] = bookEntity.Attributes["ankh_title"];
                }
               
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("set fields method", ex);
            }
        }

        private void UpdateFieldTracingMethod()
        {
            try
            {
                if (context.PostEntityImages.Contains("PostImage") && context.PostEntityImages["PostImage"] is Entity)
                {
                    Entity PostImage = context.PostEntityImages["PostImage"];
                    foreach (var attribute in PostImage.Attributes)
                    {
                        var attributeValue = attribute.Value;
                        if (attributeValue.ToString() == "Microsoft.Xrm.Sdk.EntityReference")
                        {
                            var value = PostImage.GetAttributeValue<EntityReference>("ankh_book").Name;
                            tracingService.Trace("Field name is {0} field type is {1} and it's value is {2}", attribute.Key, attributeValue.GetType(), value);
                        }
                        else
                        {
                            tracingService.Trace("Field name is {0} field type is {1} and it's value is {2}", attribute.Key, attributeValue.GetType(), attribute.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
        private void CopyFields()
        {
            try
            {
                tracingService.Trace("function copyfields");
                Entity accountEntity = new Entity("account");

                accountEntity.Attributes["ankh_primarycontactid"] = new EntityReference(entity.LogicalName, (Guid)entity.Id);
                accountEntity.Attributes["name"] = entity.GetAttributeValue<String>("ankh_accountname");
                accountEntity.Attributes["ankh_bookborrowed"] = entity.GetAttributeValue<String>("ankh_booktitle");
                accountEntity.Attributes["ankh_contactname"] = entity.GetAttributeValue<String>("ankh_name");
                service.Create(accountEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /*private void CopyFieldsUpdate()
        {
            try
            {
                tracingService.Trace("function copyfieldsUpdate");

                Guid accountEntityId = entity.GetAttributeValue<EntityReference>("ankh_primarycontactid").Id;
                Entity accountEntity = service.Retrieve("account", accountEntityId, new ColumnSet("name", "ankh_bookborrowed", "ankh_contactname"));
                if (entity.Attributes.Contains("ankh_accountname")) { accountEntity.Attributes["name"] = entity.Attributes["ankh_accountname"]; }
                if (entity.Attributes.Contains("ankh_bookborrowed")) { accountEntity.Attributes["ankh_bookborrowed"] = entity.Attributes["ankh_bookborrowed"]; }
                if (entity.Attributes.Contains("ankh_name")) { accountEntity.Attributes["ankh_contactname"] = entity.Attributes["ankh_name"]; }
                service.Update(accountEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }*/
    }
}