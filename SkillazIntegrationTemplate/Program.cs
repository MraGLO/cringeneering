using System.Net.Http.Headers;
using HttpIntegrationTemplate;

var token = args.FirstOrDefault();
if (string.IsNullOrEmpty(token))
{
    throw new Exception("Укажите токен");
}

var candidateId = args.LastOrDefault();

if (string.IsNullOrEmpty(candidateId) || candidateId == token)
{
    throw new Exception("Укажите идентификатор кандидата");
}

var client = new HttpClient
{
    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
};

var candidateService = new CandidateService(client);

Console.WriteLine("Начинаем проверку кандидата");

candidateService.CheckCandidate(candidateId);

Console.WriteLine("Проверка окончена");