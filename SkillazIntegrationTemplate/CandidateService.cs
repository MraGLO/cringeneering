using System.Data.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpIntegrationTemplate;

public class CandidateService
{
    private readonly HttpClient _httpClient;

    public CandidateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void CheckCandidate(string id)
    {
        var candidate = GetCandidateInfo(id);

        Console.WriteLine(candidate.ToString());

        var vacancy = GetVacancyInfo(candidate.VacancyId);

        Console.WriteLine(vacancy.ToString());

        if (CalculateMatching(candidate, vacancy))
        {
            AddCommentToCandidate(id, "Подходит");
            Console.WriteLine("КАНДИДАТ ПОДХОДИТ");
        }
        else
        {
            AddCommentToCandidate(id, "Не подходит");
             Console.WriteLine("КАНДИДАТ НЕ ПОДХОДИТ");
        }
    }
    
    private bool CalculateMatching(CandidateInfo candidateInfo, VacancyInfo vacancyInfo)
    {
        double count = 0;
        var mainInfo = candidateInfo.CommonCVInfo;

        foreach (var skill in vacancyInfo.Data.RequiresSkills){
            if (mainInfo.Skills.Any(x => x == skill)){
                count+=1;
            }
        }

        bool skillsComponent = (count/vacancyInfo.Data.RequiresSkills.Count) >= 0.7;
        int requestedVac = vacancyInfo.Data.WorkExperience ?? 0;
        int candidateExp = mainInfo.WorkExperience == null ? 0 : (int)mainInfo.WorkExperience.Sum(x => x.TotalMonths ?? 0);
        bool timeComponent =  Math.Abs(requestedVac - candidateExp) <= 6;
        bool citizenshipComp = false;
        bool driverLicenseComp = false;

        if (vacancyInfo.Data.Citizenship == null){
            citizenshipComp = true; 
        }
        else if (vacancyInfo.Data.Citizenship == VacancyData.Citizenships.Any.ToString()){
            citizenshipComp = true;
        }
        else if (mainInfo.Citizenship == null && vacancyInfo.Data.Citizenship == null){
            citizenshipComp = true;
        }
        else if ((mainInfo.Citizenship == null && vacancyInfo.Data.Citizenship!= null) || (mainInfo.Citizenship != null && vacancyInfo.Data.Citizenship == null)) {
            citizenshipComp = false;
        }
        else {
            citizenshipComp = mainInfo.Citizenship.Any(x => x == vacancyInfo.Data.Citizenship);
        }

        if (vacancyInfo.Data.NeedDriverLicense == null){
            driverLicenseComp = true;
        }
        else if (mainInfo.DrivingExperiences == null && vacancyInfo.Data.NeedDriverLicense == null){
            driverLicenseComp = true;
        }
        else if ((mainInfo.DrivingExperiences == null && vacancyInfo.Data.NeedDriverLicense!= null) || (mainInfo.DrivingExperiences != null && vacancyInfo.Data.NeedDriverLicense == null)) {
            driverLicenseComp = false;
        }
        else {
            driverLicenseComp = mainInfo.DrivingExperiences.Any() == vacancyInfo.Data.NeedDriverLicense;
        }
        //Console.WriteLine(skillsComponent);
        //Console.WriteLine(timeComponent);
        //Console.WriteLine(citizenshipComp);
        //Console.WriteLine(driverLicenseComp);



        return skillsComponent && timeComponent && citizenshipComp && driverLicenseComp;


        //todo: алгоритм соответствия кандидата и вакансии
        //+
    }

    public CandidateInfo GetCandidateInfo(string id)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/open-api/objects/candidates/filtered")
        {
            Content = JsonContent.Create(new CandidateInfoRequest
            {
                Ids = new[] { id }
            },  options: new() { PropertyNamingPolicy = null })
        };

        var response = _httpClient.Send(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Ошибка запроса");
        }

        var deserializedResponse = JsonSerializer.Deserialize<CandidateInfoResponse>(response.Content.ReadAsStream());

        return deserializedResponse?.Items.FirstOrDefault() ?? throw new Exception("Не найден кандидат");
    }

    private VacancyInfo GetVacancyInfo(string vacancyId)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, $"/open-api/objects/vacancies/{vacancyId}");

        var response = _httpClient.Send(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Ошибка запроса");
        }

        var deserializedResponse = JsonSerializer.Deserialize<VacancyInfo>(response.Content.ReadAsStream());

        return deserializedResponse ?? throw new Exception("Не найдена вакансия");

        //todo: используйте _httpClient для получения вакансии
    }

    private void AddCommentToCandidate(string id, string text)
    {
        var incapsulate = new CandidateNote
        {
            Text = text
        };
        var message = new HttpRequestMessage(HttpMethod.Post, $"/open-api/objects/candidates/{id}/notes")
        {
            Content = JsonContent.Create(
            new { Add = new List<CandidateNote> {incapsulate}},  
            options: new() { PropertyNamingPolicy = null })
        };

        var response = _httpClient.Send(message);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Ошибка запроса");
        }
        //todo: используйте _httpClient для отправки комментария
        //+ 
    }
}

public class CandidateInfoRequest
{
    public string[] Ids { get; set; }
    public CandidateCommonCvInfo CommonCVInfo { get; set; }
}

public class CandidateInfoResponse
{
    public List<CandidateInfo> Items { get; set; }

    public CandidateInfoResponse()
    {
        Items = new List<CandidateInfo>();
    }
}

public class CandidateInfo
{
    public string VacancyId { get; set; }
    public List<CandidateNote> Notes { get; set; }
    public CandidateCommonCvInfo  CommonCVInfo {get; set;}

    public override string ToString()
    {
        return VacancyId + "\n" + 
           "Заметки: "  + string.Join(", " , Notes) + "\n" +
            CommonCVInfo.ToString();
        ;
    }

    //todo: добавьте остальные поля на основе swagger
    // +
}

public class CandidateCommonCvInfo
{
    public List<string>? Citizenship { get; set; }
    public List<CandidateWorkExperience>? WorkExperience {get; set; }
    public List<string>? Skills {get; set;}
    public List<CandidateDrivingExpirience>? DrivingExperiences {get; set; }

    public override string ToString()
    {
        return 
        "Умения: "  + string.Join(", " , Skills ?? new List<string>()) + "\n" +
        "Гражданства: " + string.Join(", ", Citizenship ?? new List<string>()) + "\n" +
        "Опыт работы: " + (WorkExperience == null ? "" : string.Join(", ", WorkExperience.Select( x => x.CompanyName + " " + (x.TotalMonths ?? 0)))) + "\n" +
        "Водительские права: " + (DrivingExperiences == null ? "" : string.Join(", ", DrivingExperiences.Select( x => x.HasPersonalCar + " " + x.DrivingLicense))) + "\n";

    }

    //todo: добавьте остальные поля на основе swagger
}

public class CandidateWorkExperience
{
    public string? CompanyName { get; set; }
    public int? TotalMonths { get; set; }
}
public class CandidateDrivingExpirience
{
    public bool? HasPersonalCar { get; set; }
    public string? DrivingLicense { get; set; }
}

public class CandidateNote
{
    public string? Id { get; set; }
    public string? Text { get; set; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }

    public override string ToString()
    {
        return "id: " + Id ?? " " + ", " + Text ?? "";
    }

}

public class VacancyInfo
{
    public string? Name {get; set; }
    public VacancyData? Data {get; set; }

    public override string ToString()
    {
        return "Имя: " + (Name ?? " ") + "\n" +
        "Данные: \n" + (Data == null ? " " : Data.ToString()) ;  
    }

    //todo: add properties for vacancy
    //+
}

public class VacancyData {

    public enum Citizenships
    {
        Rus = 1,
        Kz = 2,
        Any = 3
    } 

    [JsonPropertyName("ExtraData.RequiredSkills")]
    public List<string>? RequiresSkills {get; set;}
    [JsonPropertyName("ExtraData.WorkExperience")]
    public int? WorkExperience;
    [JsonPropertyName("ExtraData.Citizenship")]
    public string? Citizenship;
    [JsonPropertyName("ExtraData.NeedDriverLicense")]
    public bool? NeedDriverLicense;

    public override string ToString()
    {
        return 
        "Необходимо: " + (RequiresSkills == null ? " " : string.Join(", ", RequiresSkills)) + "\n" +
        "Опыт работы: " + (WorkExperience ?? 0).ToString() + "\n" +
        "Гражданство: " + (Citizenship ?? "нет") + "\n" +
        "Вод. права: " + NeedDriverLicense ?? "Не указано" + "\n";
    }



}