using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LiftStatus.Data;
using LiftStatus.Domain;
using System.Collections.Generic;

namespace LiftStatus
{

    public static class GetAll
    {
        [FunctionName("Get")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP Request for current lift status");

            var statusRepository = new DocumentDbRepository<Lift>("status");
            var liftStatus = await statusRepository.GetItemsAsync( x => true, "/Location");

            if (liftStatus == null)
                return new NotFoundResult();

            return new OkObjectResult(liftStatus);

        }
    }

    public static class GetSingle
    {
        [FunctionName("GetSingle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req,
            ILogger log)
        {

            var liftId = req.Query["lift-id"];
            var locationId = req.Query["location"];

            var statusRepository = new DocumentDbRepository<Lift>("status");
            var singleLift = await statusRepository.GetItemAsync(liftId, locationId);

            if (singleLift == null)
                return new NotFoundResult();

            return new OkObjectResult(singleLift);

        }

    }

}
