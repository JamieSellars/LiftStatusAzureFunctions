using System;
using System.Collections.Generic;
using System.Linq;
using LiftStatus.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LiftStatus
{
    public static class SendOpenNotification
    {
        [FunctionName("SendOpenNotification")]
        public static void Run([CosmosDBTrigger(
            databaseName: "lift-notify",
            collectionName: "status",
            LeaseDatabaseName = "lift-notify",
            LeaseCollectionName = "StatusUpdateTrigger",
            CreateLeaseCollectionIfNotExists = true,
            ConnectionStringSetting = "DbConnString")]

        IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                // Mock changed status update
                // Todo add notification hub
                var liftStatus = JsonConvert.DeserializeObject<Lift>(input.First().ToString());

                if (liftStatus.Status == "Closed")
                {
                    log.LogInformation("Lift Status was changed to closed");
                }

                if (liftStatus.Status == "Open")
                {
                    log.LogInformation("Lift Status was changed to open");
                }

            }
        }
    }
}
