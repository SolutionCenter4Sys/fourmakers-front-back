using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Linkedin
{
    public class Artifact
    {
        [JsonPropertyName("expiresAt")]
        public object expiresAt { get; set; }

        [JsonPropertyName("fileIdentifyingUrlPathSegment")]
        public string fileIdentifyingUrlPathSegment { get; set; }

        [JsonPropertyName("height")]
        public int height { get; set; }

        [JsonPropertyName("width")]
        public int width { get; set; }
    }

    public class BasicLocation
    {
        [JsonPropertyName("countryCode")]
        public string countryCode { get; set; }
    }

    public class Certification
    {
        [JsonPropertyName("authority")]
        public string authority { get; set; }

        [JsonPropertyName("company")]
        public Company company { get; set; }

        [JsonPropertyName("companyUrn")]
        public string companyUrn { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("timePeriod")]
        public TimePeriod timePeriod { get; set; }

        [JsonPropertyName("displaySource")]
        public string displaySource { get; set; }

        [JsonPropertyName("licenseNumber")]
        public string licenseNumber { get; set; }

        [JsonPropertyName("url")]
        public string url { get; set; }
    }

    public class ComLinkedinCommonVectorImage
    {
        [JsonPropertyName("artifacts")]
        public List<Artifact> artifacts { get; set; }

        [JsonPropertyName("rootUrl")]
        public string rootUrl { get; set; }
    }

    public class Company
    {
        [JsonPropertyName("active")]
        public bool active { get; set; }

        [JsonPropertyName("dashCompanyUrn")]
        public string dashCompanyUrn { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("logo")]
        public Logo logo { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("objectUrn")]
        public string objectUrn { get; set; }

        [JsonPropertyName("showcase")]
        public bool showcase { get; set; }

        [JsonPropertyName("trackingId")]
        public string trackingId { get; set; }

        [JsonPropertyName("universalName")]
        public string universalName { get; set; }

        [JsonPropertyName("employeeCountRange")]
        public EmployeeCountRange employeeCountRange { get; set; }

        [JsonPropertyName("industries")]
        public List<string> industries { get; set; }

        [JsonPropertyName("miniCompany")]
        public MiniCompany miniCompany { get; set; }
    }

    public class Education
    {
        [JsonPropertyName("degreeName")]
        public string degreeName { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("fieldOfStudy")]
        public string fieldOfStudy { get; set; }

        [JsonPropertyName("school")]
        public School school { get; set; }

        [JsonPropertyName("schoolName")]
        public string schoolName { get; set; }

        [JsonPropertyName("schoolUrn")]
        public string schoolUrn { get; set; }

        [JsonPropertyName("timePeriod")]
        public TimePeriod timePeriod { get; set; }
    }

    public class EmployeeCountRange
    {
        [JsonPropertyName("end")]
        public int end { get; set; }

        [JsonPropertyName("start")]
        public int start { get; set; }
    }

    public class EndDate
    {
        [JsonPropertyName("month")]
        public int? month { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }
    }

    public class Experience
    {
        [JsonPropertyName("company")]
        public Company company { get; set; }

        [JsonPropertyName("companyLogoUrl")]
        public string companyLogoUrl { get; set; }

        [JsonPropertyName("companyName")]
        public string companyName { get; set; }

        [JsonPropertyName("companyUrn")]
        public string companyUrn { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("geoLocationName")]
        public string geoLocationName { get; set; }

        [JsonPropertyName("locationName")]
        public string locationName { get; set; }

        [JsonPropertyName("timePeriod")]
        public TimePeriod timePeriod { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("geoUrn")]
        public string geoUrn { get; set; }

        [JsonPropertyName("region")]
        public string region { get; set; }
    }

    public class GeoLocation
    {
        [JsonPropertyName("geoUrn")]
        public string geoUrn { get; set; }
    }

    public class Language
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("proficiency")]
        public string proficiency { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("basicLocation")]
        public BasicLocation basicLocation { get; set; }
    }

    public class Logo
    {
        [JsonPropertyName("com.linkedin.common.VectorImage")]
        public ComLinkedinCommonVectorImage comlinkedincommonVectorImage { get; set; }
    }

    public class MiniCompany
    {
        [JsonPropertyName("active")]
        public bool active { get; set; }

        [JsonPropertyName("dashCompanyUrn")]
        public string dashCompanyUrn { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("logo")]
        public Logo logo { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("objectUrn")]
        public string objectUrn { get; set; }

        [JsonPropertyName("showcase")]
        public bool showcase { get; set; }

        [JsonPropertyName("trackingId")]
        public string trackingId { get; set; }

        [JsonPropertyName("universalName")]
        public string universalName { get; set; }
    }

    public class Profile
    {
        [JsonPropertyName("certifications")]
        public List<Certification> certifications { get; set; }

        [JsonPropertyName("education")]
        public List<Education> education { get; set; }

        [JsonPropertyName("elt")]
        public bool elt { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("experience")]
        public List<Experience> experience { get; set; }

        [JsonPropertyName("firstName")]
        public string firstName { get; set; }

        [JsonPropertyName("geoCountryName")]
        public string geoCountryName { get; set; }

        [JsonPropertyName("geoCountryUrn")]
        public string geoCountryUrn { get; set; }

        [JsonPropertyName("geoLocation")]
        public GeoLocation geoLocation { get; set; }

        [JsonPropertyName("geoLocationBackfilled")]
        public bool geoLocationBackfilled { get; set; }

        [JsonPropertyName("geoLocationName")]
        public string geoLocationName { get; set; }

        [JsonPropertyName("headline")]
        public string headline { get; set; }

        [JsonPropertyName("honors")]
        public List<object> honors { get; set; }

        [JsonPropertyName("industryName")]
        public string industryName { get; set; }

        [JsonPropertyName("industryUrn")]
        public string industryUrn { get; set; }

        [JsonPropertyName("languages")]
        public List<Language> languages { get; set; }

        [JsonPropertyName("lastName")]
        public string lastName { get; set; }

        [JsonPropertyName("location")]
        public Location location { get; set; }

        [JsonPropertyName("locationName")]
        public string locationName { get; set; }

        [JsonPropertyName("member_urn")]
        public string member_urn { get; set; }

        [JsonPropertyName("profile_id")]
        public string profile_id { get; set; }

        [JsonPropertyName("profile_urn")]
        public string profile_urn { get; set; }

        [JsonPropertyName("projects")]
        public List<object> projects { get; set; }

        [JsonPropertyName("public_id")]
        public string public_id { get; set; }

        [JsonPropertyName("publications")]
        public List<object> publications { get; set; }

        [JsonPropertyName("skills")]
        public List<Skill> skills { get; set; }

        [JsonPropertyName("student")]
        public bool student { get; set; }

        [JsonPropertyName("summary")]
        public string summary { get; set; }

        [JsonPropertyName("urn_id")]
        public string urn_id { get; set; }

        [JsonPropertyName("volunteer")]
        public List<Volunteer> volunteer { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("phone")]
        public string phone { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("profile")]
        public Profile profile { get; set; }

        [JsonPropertyName("skills")]
        public List<Skill> skills { get; set; }
    }
    
    public class RootIA
    {
        [JsonPropertyName("profile")]
        public Profile profile { get; set; }
        public string? UrlLinkedin { get; set; }
        [JsonPropertyName("skills")]
        public SkillsHabilidades habilidades { get; set; }
    }

    public class SkillsHabilidades
    {
        [JsonPropertyName("hardSkills")]
        public List<string> hardSkills { get; set; }
        [JsonPropertyName("softSkills")]
        public List<string> softSkills { get; set; }
        [JsonPropertyName("dominioNegocios")]
        public List<string> dominioNegocios { get; set; }
        [JsonPropertyName("metodologias")]
        public List<string> metodologias { get; set; }
    }

    public class School
    {
        [JsonPropertyName("active")]
        public bool active { get; set; }

        [JsonPropertyName("entityUrn")]
        public string entityUrn { get; set; }

        [JsonPropertyName("logoUrl")]
        public string logoUrl { get; set; }

        [JsonPropertyName("objectUrn")]
        public string objectUrn { get; set; }

        [JsonPropertyName("schoolName")]
        public string schoolName { get; set; }

        [JsonPropertyName("trackingId")]
        public string trackingId { get; set; }
    }

    public class Skill
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
    }

    public class StartDate
    {
        [JsonPropertyName("month")]
        public int? month { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }
    }

    public class TimePeriod
    {
        [JsonPropertyName("startDate")]
        public StartDate? startDate { get; set; }

        [JsonPropertyName("endDate")]
        public EndDate endDate { get; set; }
    }

    public class Volunteer
    {
        [JsonPropertyName("cause")]
        public string cause { get; set; }

        [JsonPropertyName("company")]
        public Company company { get; set; }

        [JsonPropertyName("companyName")]
        public string companyName { get; set; }

        [JsonPropertyName("companyUrn")]
        public string companyUrn { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("role")]
        public string role { get; set; }

        [JsonPropertyName("timePeriod")]
        public TimePeriod timePeriod { get; set; }
    }
}