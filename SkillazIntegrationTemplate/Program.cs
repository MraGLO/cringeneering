using System.Net.Http.Headers;
using HttpIntegrationTemplate;

var token = args.FirstOrDefault();
if (string.IsNullOrEmpty(token))
{
    throw new Exception("Укажите токен");
}

var candidateId = "<тут укажите айди кандидата>";

if (string.IsNullOrEmpty(candidateId) || candidateId == token)
{
    throw new Exception("Укажите идентификатор кандидата");
}

var client = new HttpClient
{
    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
    BaseAddress = new Uri("https://api-feature-configurator.dev.skillaz.ru/")
    
};

var candidateService = new CandidateService(client);

Console.WriteLine("Начинаем проверку кандидата");

candidateService.CheckCandidate(candidateId);

Console.WriteLine("Проверка окончена");