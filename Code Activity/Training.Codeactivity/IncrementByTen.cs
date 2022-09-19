using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.Codeactivity
{
    public class IncrementByTen : CodeActivity
    {
        [Input("Number")]
        public InArgument<int> number { get; set; }

        [Output("result")]
        public OutArgument<int> result { get; set; }

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
                int number = this.number.Get(executionContext);
                int incrementedNumber = ((int)(number - (number * (0.1))));
                tracingService.Trace("workflow output {0}", incrementedNumber);
                result.Set(executionContext, incrementedNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}