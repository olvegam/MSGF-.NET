using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client;
using MsgFoundation.Data;
using MsgFoundation.Models;

namespace MsgFoundation.Functions
{
    public class ObservationFunctions
    {

        public static async Task CreateObservation(ExternalTaskInfo task, MsgFoundationContext dbcontext, CamundaClient camunda)
        {

            Dictionary<string, VariableValue> variables = await camunda.Executions[task.ExecutionId].LocalVariables.GetAll();
            string fullname = variables["responsibleSpouse"].GetValue<string>();
            string email = variables["email"].GetValue<string>();
            string phone = variables["phone"].GetValue<string>();
            string observationText = variables["observationText"].GetValue<string>();



            Observation observation = new Observation
            {
                Id = Guid.NewGuid(),
                Fullname = fullname.ToString(),
                Email = email.ToString(),
                Phone = phone.ToString(),
                ObservationText = observationText.ToString(),
                Date = DateTime.UtcNow,
            };

            dbcontext.Observations.Add(observation);
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

            await camunda.ExternalTasks[task.Id].Complete(request);

            Console.Write("Observacion creada se envio un email al responsable de la pareja " + email + " con las siguiente observacion " + observationText);
        }

    }
}
