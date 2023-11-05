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
        //todo: calculate matching for candidate and vacancy
        return true;
    }

    public CandidateInfo GetCandidateInfo(string id)
    {
        //todo: use _httpClient for getting candidate
        return new CandidateInfo();
    }

    private VacancyInfo GetVacancyInfo(string vacancyId)
    {
        //todo: use _httpClient for getting vacancy
        return new VacancyInfo();
    }

    private void AddCommentToCandidate(string id, string text)
    {
        //todo: use _httpClient for sending comment 
    }
}

public class CandidateInfo
{
    public string VacancyId { get; set; }
    public List<CandidateNote> Notes { get; set; }
    //todo: add properties for candidate
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