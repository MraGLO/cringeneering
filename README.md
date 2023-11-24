## Задание
Необходимо доработать консольное приложение - реализовать методы получения кандидатов и вакансий по API, а также проверку соответствия кандидата и его вакансии.
#### Поля вакансии
- `Name` - название вакансии
- `ExtraData.RequiredSkills` - требуемые навыки, массив строк
- `ExtraData.WorkExperience` - требуемый опыт работы, число
- `ExtraData.Citizenship` - требуемое гражданство, строка, одно из значений
    - `Rus`
    - `Kz`
    - `Any`
- `ExtraData.NeedDriverLicense` - требуется наличие водительских прав
#### Поля кандидата
- `CommonCandidateInfo.Skills` - навыки, массив строк
- `CommonCandidateInfo.Citizenship` - гражданства кандидата, массив строк
- `CommonCandidateInfo.WorkExperience` - опыт работы, массив объектов
- `CommonCandidateInfo.DrivingExperiences` - опыт вождения, массив объектов

### Шаблон приложения
В классе `CandidateService` необходимо реализовать все методы с пометкой `//todo:`
- `GetCandidateInfo` - получение кандидата по API
    - POST `/open-api/objects/candidates/filtered`  
      Тело запроса:
```json  
{
  "Ids": [
    "string"
  ]
} 
```  
Тело ответа:
```json  
{
  "Items": [
    {
      "Id": "string",
      "FirstName": "string",
      "MiddleName": "string",
      "LastName": "string",
      "ContactPhoneNumber": "string",
      "ContactEmail": "string",
      "CommonCVInfo": {
        "BirthDate": "2023-11-02T05:19:53.652Z",
        "Skills": [
          "string"
        ],
        "Citizenship": [
          "string"
        ],
        "WorkExperience": [
          {
            "CompanyName": "string",
            "Position": "string",
            "StartDate": "2023-11-02T05:19:53.652Z",
            "EndDate": "2023-11-02T05:19:53.652Z",
            "TotalMonths": 0,
            "Description": "string",
            "Industries": "string",
            "City": "string",
            "EmploymentType": "Any"
          }
        ],
        "City": "string",
        "Country": "string",
        "DrivingExperiences": [
          {
            "HasPersonalCar": true,
            "DrivingLicense": "Undefined"
          }
        ]
      },
      "VacancyId": "string",
      "Notes": [
        {
          "Id": "string",
          "Text": "string",
          "CreatedById": "string",
          "CreatedByName": "string",
          "CreatedByClientId": "string",
          "UpdatedById": "string",
          "UpdatedByName": "string",
          "UpdatedByClientId": "string",
          "IsInternal": true,
          "CreatedAt": "2023-11-02T05:19:53.652Z",
          "UpdatedAt": "2023-11-02T05:19:53.652Z",
          "NotifyAt": "2023-11-02T05:19:53.652Z",
          "RefCandidateId": "string",
          "CandidateStatusId": "string"
        }
      ]
    }
  ],
  "NextPage": 0,
  "TotalPages": 0,
  "TotalItems": 0
}
```  

- `GetVacancyInfo` - получение вакансии по API
    - GET `/open-api/objects/vacancies/{vacancyId}`  
      Тело ответа:
```json  
{
  "Id": "string",
  "Name": "string",
  "IsActive": true,
  "FunnelId": "string",
  "Data": {
    "Name": "string",
    "FunnelId": "string",
    "ExtraData.RequiredSkills": [
      "string"
    ],
    "ExtraData.WorkExperience": 0,
    "ExtraData.Citizenship": "string",
    "ExtraData.NeedDriverLicense": true
  }
}
```  
- `CalculateMatching` - алгоритм соответствия кандидата вакансии
- Критерии соответствия вакансии и кандидата
-     Навыки - пересечение навыков от 70%
    - Требуемый опыт работы - погрешность в 6 месяцев
    - Требуемое гражданство - если не соответствует, то кандидат не подходит
    - Наличие водительских прав - если не соответствует, то кандидат не подходит
- `AddCommentToCandidate` - добавление комментария кандидату по API
- `POST /open-api/objects/candidates/{candidateId}/notes`  
      Тело запроса:
```json  
{
  "Add": [
    {
      "Text": "string"
    }
  ]
}
```
