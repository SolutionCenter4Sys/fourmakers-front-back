using System;

namespace DataTransferObject.Domain.SRS.Onboarding
{
    public class OnboardingParam
    {
        public int Candidate_id { get; set; }
        public DateTime Date_start { get; set; }
        public DateTime Project_date_start { get; set; }
        public string Project_name { get; set; }
        public int Oboarding_group_id { get; set; }
    }
}