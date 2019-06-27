using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiftStatus.Domain
{
    public class Lift
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string TimeStamp { get; set; }

        public override bool Equals(object obj)
        {


            if (!(obj is Lift other))
                return false;

            if (Id != other.Id ||
                Name != other.Name ||
                Status != other.Status ||
                StatusId != other.StatusId ||
                LocationId != other.LocationId ||
                Location != other.Location ||
                OpenTime != other.OpenTime ||
                CloseTime != other.CloseTime ||
                TimeStamp != other.TimeStamp)
                return false;

            return true;

        }

    }
}
