using System.Net.Http.Json;
using System.Text.Json;

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
        var vacancy = GetVacancyInfo(candidate.VacancyId);

        if (CalculateMatching(candidate, vacancy))
        {
            AddCommentToCandidate(id, "Подходит");
        }
        else
        {
            AddCommentToCandidate(id, "Не подходит");
        }
    }
    
    private bool CalculateMatching(CandidateInfo candidateInfo, VacancyInfo vacancyInfo)
    {
        //todo: алгоритм соответствия кандидата и вакансии
        return true;
    }

    public CandidateInfo GetCandidateInfo(string id)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "/open-api/objects/candidates/filtered")
        {
            Content = JsonContent.Create(new CandidateInfoRequest
            {
                Ids = new[] { id }
            }, options: new() { PropertyNamingPolicy = null })
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
        //todo: используйте _httpClient для получения вакансии
        return new VacancyInfo();
    }

    private void AddCommentToCandidate(string id, string text)
    {
        //todo: используйте _httpClient для отправки комментария 
    }
}

public class CandidateInfoRequest
{
    public string[] Ids { get; set; }
    public CandidateCommonCvInfo CommonCVInfo { get; set; }
}

public class CandidateInfoResponse
{
    public CandidateInfo[] Items { get; set; }
}

public class CandidateInfo
{
    public string VacancyId { get; set; }
    public List<CandidateNote> Notes { get; set; }
    //todo: добавьте остальные поля на основе swagger
}

public class CandidateCommonCvInfo
{
    public string[] Citizenship { get; set; }
    public CandidateWorkExperience[] WorkExperience { get; set; }
    //todo: добавьте остальные поля на основе swagger
}

public class CandidateWorkExperience
{
    public string CompanyName { get; set; }
    private int TotalMonths { get; set; }
}

public class CandidateNote
{
    public string Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VacancyInfo
{
    //todo: add properties for vacancy
}