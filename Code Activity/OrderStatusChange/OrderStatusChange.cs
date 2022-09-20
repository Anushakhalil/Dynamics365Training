using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStatusChange
{
    public class OrderStatusChange : CodeActivity
    {

        [Output("setParent")]
        [ReferenceTarget("ankh_orderproduct")]
        public OutArgument<EntityReference> SetParent { get; set; }

        ITracingService tracingService;

        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                tracingService.Trace("primaryEntityName: " + context.PrimaryEntityName);

                // getting data of current Order entity
                Entity currentEntity = service.Retrieve(context.PrimaryEntityName, context.PrimaryEntityId, new ColumnSet(true));
          
                //  creating new order entity
                if (context.PrimaryEntityName == "ankh_order")
                {
                    Entity newOrderEntity = new Entity("ankh_order");

                    // Setting fields of new order entity
                    newOrderEntity.Attributes["ankh_name"] = currentEntity["ankh_name"];
                    newOrderEntity.Attributes["ankh_contact"] = currentEntity["ankh_contact"];
                    newOrderEntity.Attributes["ankh_deliverytime"] = currentEntity["ankh_deliverytime"];
                    newOrderEntity.Attributes["ankh_orderdate"] = currentEntity["ankh_orderdate"];

                    // setting new record status to new order
                    OptionSetValue picklist = new OptionSetValue(0);
                    newOrderEntity.Attributes["ankh_orderstatus"] = picklist;

                    // setting Lookup of current order entity to newly created entity to make parent
                    SetParent.Set(executionContext, newOrderEntity.Attributes["ankh_parentorder"] = new EntityReference(context.PrimaryEntityName, context.PrimaryEntityId));
                    tracingService.Trace("current order entity data set to new order entity ");

                    service.Create(newOrderEntity);
                    tracingService.Trace("new order entity created ");
                }

                //  creating new order product entity
                else if (context.PrimaryEntityName == "ankh_orderproduct")
                {
                    // integer value of remaining product field
                    int value = currentEntity.GetAttributeValue<int>("ankh_remainingquantity");

                    if (value > 0) 
                    {
                        Entity newOrderProductEntity = new Entity("ankh_orderproduct");

                        // Setting fields of new order entity
                        newOrderProductEntity.Attributes["ankh_name"] = currentEntity["ankh_name"];

                        newOrderProductEntity.Attributes["ankh_order"] = currentEntity["ankh_order"];
                        newOrderProductEntity.Attributes["ankh_product"] = currentEntity["ankh_product"];
                        newOrderProductEntity.Attributes["ankh_orderedquantity"] = currentEntity["ankh_orderedquantity"];
                        newOrderProductEntity.Attributes["ankh_deliveredquantity"] = currentEntity["ankh_deliveredquantity"];
                        newOrderProductEntity.Attributes["ankh_remainingquantity"] = currentEntity["ankh_remainingquantity"];
                        
                        // setting new record status to new order
                        OptionSetValue picklist = new OptionSetValue(0);
                        newOrderProductEntity.Attributes["ankh_orderproductstatus"] = picklist;
                        tracingService.Trace("current order product entity data set to new order product entity ");

                        service.Create(newOrderProductEntity);
                        tracingService.Trace("new order entity created ");
                    }
                    else
                    {
                        tracingService.Trace("value is not greater than 0");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}