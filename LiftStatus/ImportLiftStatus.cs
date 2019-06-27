using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LiftStatus.Data;
using LiftStatus.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LiftStatus
{
    public static class ImportLiftStatus
    {

        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("ImportLiftStatus")]
        public static async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {

            // Instantiate db connection
            var liftRepository = new DocumentDbRepository<Lift>("status");
            var lifts = await GetLiftStatus();


            foreach(var lift in lifts)
            {
                // Get the existing document for this lift
                var currentItem = await liftRepository.GetItemAsync(lift.Id, lift.Location);

                // Create the document if the lift does not exist
                if (currentItem == null)                    
                    await liftRepository.CreateItemAsync(lift, lift.Location);

                // Check if the lift status has changed and update document
                if (currentItem != null && !currentItem.Equals(lift))
                    await liftRepository.UpdateItemAsync(lift.Id, lift, lift.Location);

            }

            
        }
        
        private static async Task<List<Lift>> GetLiftStatus()
        {

            var request = await _httpClient.GetAsync("https://mountainops.perisher.com.au/api/public/lift-status");
            var response = await request.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(response))
                return null;

            return JsonConvert.DeserializeObject<List<Lift>>(response);

        }

    }
}
