using System.Collections.Generic;
using Newtonsoft.Json;

namespace DinePlan.Custom.TableCheck.Model
{
    public class CustomerModel
    {
        public string Id { get; set; }
        public string Ref { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("kanji_first_name")]
        public string KanjiFirstName { get; set; }
        [JsonProperty("kanji_last_name")]
        public string KanjiLastName { get; set; }
        public string Sex { get; set; }
        public string Title { get; set; }
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        public string Division { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Likes { get; set; }
        public List<string> Dislikes { get; set; }
        public List<string> Allergries { get; set; }
        public List<string> Phones { get; set; }
        public List<string> Emails { get; set; }
    }
}
