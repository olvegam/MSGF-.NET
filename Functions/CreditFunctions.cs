using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client;
using MsgFoundation.Data;
using MsgFoundation.Models;

namespace MsgFoundation.Functions
{
    public class CreditFunctions
    {
        public static async Task CreateCredit(ExternalTaskInfo task, MsgFoundationContext dbcontext, CamundaClient camunda)
        {
            Dictionary<string, VariableValue> variables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();
            string idUser = variables["idUser"].GetValue<string>();
            string authorizeCredit = variables["authorizeCredit"].GetValue<string>();
            double propertyValue = variables["propertyValue"].GetValue<double>();
            double valueSaved = variables["valueSaved"].GetValue<double>();
            string legalizationInstruccions = variables["legalizationInstructions"].GetValue<string>();
            double valueCredit = propertyValue - valueSaved;
            string disbursed = "no";

            //if (authorizeCredit == "no") { authorizeCredit = "Rejected"; }
            authorizeCredit = authorizeCredit == "yes" ? "approved" : "rejected";

            Credit credit = new Credit
            {
                Id = Guid.NewGuid(),
                IdUser = idUser,
                CreditValue = valueCredit,
                Date = DateTime.UtcNow,
                StatusCredit = authorizeCredit,
                Disbursed = disbursed,

            };

            dbcontext.Credits.Add(credit);
            dbcontext.SaveChanges();

            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>();
            request.Variables.Add("idCredit", VariableValue.FromObject(credit.Id));
            request.Variables.Add("valueCredit", VariableValue.FromObject(credit.CreditValue));


            await camunda.ExternalTasks[task.Id].Complete(request);

            if (authorizeCredit == "rejected")
            {
                Console.Write("Your credit has been for a value of " + valueCredit + " has been " + authorizeCredit);
            }
            else if (authorizeCredit == "approved")
            {
                Console.Write("Your credit has been for a value of " + valueCredit + " has been " + authorizeCredit + legalizationInstruccions);
            }

        }


        public static async Task ModifyDisbursed(ExternalTaskInfo task, MsgFoundationContext dbcontext, CamundaClient camunda)
        {
            Dictionary<string, VariableValue> variables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();
            string idCredit = variables["idCredit"].GetValue<string>();
            string disbursed = "yes";


            Credit credit = dbcontext.Credits.Find(Guid.Parse(idCredit));
            credit.Disbursed = disbursed;
            dbcontext.SaveChanges();
            

            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>();
            request.Variables.Add("disburshed", VariableValue.FromObject(disbursed));


            await camunda.ExternalTasks[task.Id].Complete(request);

        }

        //method CreditCompletionReport
        public static async Task CreditCompletionReport(ExternalTaskInfo task, MsgFoundationContext dbcontext, CamundaClient camunda)
        {
            Dictionary<string, VariableValue> variables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();
            string idCredit = variables["idCredit"].GetValue<string>();
            string disbursed = variables["idUser"].GetValue<string>();
            string fullNameSpouse1 = variables["fullNameSpouse1"].GetValue<string>();
            string fullNameSpouse2 = variables["fullNameSpouse2"].GetValue<string>();
            string valueCredit = variables["valueCredit"].GetValue<string>();

            FetchExternalTasks fetch = new FetchExternalTasks();
            fetch.WorkerId = "worker";
            fetch.MaxTasks = 1;
            fetch.UsePriority = true;
            fetch.Topics = new List<FetchExternalTaskTopic>();
            fetch.Topics.Add(new FetchExternalTaskTopic(task.TopicName, 1));

            List<LockedExternalTask> lockedTasks = await camunda.ExternalTasks.FetchAndLock(fetch);

            CompleteExternalTask request = new CompleteExternalTask();
            request.WorkerId = "worker";
            request.Variables = new Dictionary<string, VariableValue>();
           
            await camunda.ExternalTasks[task.Id].Complete(request);

            Console.Write("The credit with id " + idCredit + " has been disbursed to " + fullNameSpouse1 + " and " + fullNameSpouse2 + " for a value of " + valueCredit + " has been disbursed therefore the credit is completed");

        }
    }
}
