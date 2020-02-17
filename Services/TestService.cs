using System.Net.Http;
using System.Text.Json;

namespace UserAgentDetector.Services
{
    public class TestService : ITestService
    {
        private readonly HttpClient _client;

        public TestService (IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient();
        }

        private const int TEST_COUNT = 6;

        private readonly string[] TEST_ARRAY = new string[TEST_COUNT]
        {
            "Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19",
            "Mozilla/5.0 (Linux; Android 5.0.2; SAMSUNG SM-T530 Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/3.2 Chrome/38.0.2125.102 Safari/537.36",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/603.1.23 (KHTML, like Gecko) Version/10.0 Mobile/14E5239e Safari/602.1",
            "Mozilla/5.0 (iPad; CPU OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/600.7.12 (KHTML, like Gecko) Version/8.0.7 Safari/600.7.12",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; Win64; x64; Trident/7.0; .NET4.0E; .NET4.0C; .NET CLR 3.5.30729; .NET CLR 2.0.50727; .NET CLR 3.0.30729; Tablet PC 2.0; Microsoft Outlook 15.0.4893; ms-office; MSOffice 15)",
        };
           
        private readonly string[] EXPECTED_ARRAY = new string[TEST_COUNT]
        {
            @"
            {
                ""os_family"": ""Android"",
                ""os_version"": ""4.0.4"",
                ""os_meta"": 
                {
                    ""name"": ""Android"",
                    ""short_name"": ""AND"",
                    ""version"": ""4.0.4"",
                    ""platform"": """"
                },
                ""device"": 
                {
                    ""is_mobile"": true,
                    ""is_tablet"": false,
                    ""is_desktop"": false
                }
            }",
            @"
            {
                ""os_family"": ""Android"",
                ""os_version"": ""5.0.2"",
                ""os_meta"": 
                {
                    ""name"": ""Android"",
                    ""short_name"": ""AND"",
                    ""version"": ""5.0.2"",
                    ""platform"": """"
                },
                ""device"": 
                {
                    ""is_mobile"": true,
                    ""is_tablet"": true,
                    ""is_desktop"": false
                }
            }",
            @"
            {
                ""os_family"": ""iOS"",
                ""os_version"": ""10.3"",
                ""os_meta"": 
                {
                    ""name"": ""iOS"",
                    ""short_name"": ""IOS"",
                    ""version"": ""10.3"",
                    ""platform"": """"
                },
                ""device"": 
                {
                    ""is_mobile"": true,
                    ""is_tablet"": false,
                    ""is_desktop"": false
                }            
            }",
            @"
            {
                ""os_family"": ""iOS"",
                ""os_version"": ""12.2"",
                ""os_meta"": 
                {
                    ""name"": ""iOS"",
                    ""short_name"": ""IOS"",
                    ""version"": ""12.2"",
                    ""platform"": """"
                },
                ""device"": 
                {
                    ""is_mobile"": true,
                    ""is_tablet"": true,
                    ""is_desktop"": false
                }
            }",
            @"
            {
                ""os_family"": ""Mac"",
                ""os_version"": ""10.10.4"",
                ""os_meta"": 
                {
                    ""name"": ""Mac"",
                    ""short_name"": ""MAC"",
                    ""version"": ""10.10.4"",
                    ""platform"": """"
                },
                ""device"": 
                {
                    ""is_mobile"": false,
                    ""is_tablet"": false,
                    ""is_desktop"": true
                }            
            }",
            @"
            {
                ""os_family"": ""Windows"",
                ""os_version"": ""8.1"",
                ""os_meta"": 
                {
                    ""name"": ""Windows"",
                    ""short_name"": ""WIN"",
                    ""version"": ""8.1"",
                    ""platform"": ""x64""
                },
                ""device"": 
                {
                    ""is_mobile"": false,
                    ""is_tablet"": false,
                    ""is_desktop"": true
                }            
            }",
        };

        public string runTest()
        {
            for (int i = 0; i < TEST_COUNT; i++)
            {
                var responce = getResponce(TEST_ARRAY[i]);
                if (!responce.IsSuccessStatusCode)
                {
                    return $"\"{responce.RequestMessage}\" has been reterned \"{responce.StatusCode}\"";
                }
                var resValidate = validate(responce.Content.ReadAsStringAsync().Result, EXPECTED_ARRAY[i]);
                if (!resValidate.ok)
                {
                    return resValidate.err + $" in test {i + 1}";
                }
            }

            return "Successfull";
        }

        public HttpResponseMessage getResponce(string ua)
        {
            return _client.GetAsync($"https://localhost:44301/useragent?ua={ua}").Result;       
        }

        public (bool ok, string err) validate(string responce, string expected)
        {
            (bool ok, string err) result = (true, null);
            JsonDocument expectedJson = null;
            JsonDocument responceJson = null;
            try
            {
                expectedJson = JsonDocument.Parse(expected);
                responceJson = JsonDocument.Parse(responce);
            }
            catch (JsonException e)
            {
                result.ok = false;
                result.err = e.Message;
            }
            if (result.ok)
            {
                result = validateJson(responceJson.RootElement, expectedJson.RootElement);
            }
            return result;
        }

        private (bool ok, string err) validateJson(JsonElement responce, JsonElement expected)
        {
            (bool ok, string err) result = (true, null);
            foreach (var expectedProp in expected.EnumerateObject())
            {
                if (responce.TryGetProperty(expectedProp.Name, out var responceElem))
                {
                    if (responceElem.ValueKind == expectedProp.Value.ValueKind)
                    {
                        if (responceElem.ValueKind == JsonValueKind.Object)
                        {
                            result = validateJson(responceElem, expectedProp.Value);
                            if (!result.ok)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (!valueEquals(expectedProp.Value, responceElem))
                            {
                                result.ok = false;
                                result.err = $"Property with name \"{expectedProp.Name}\" has other value (expected: {expectedProp.Value}, actaul: {responceElem})";
                                break;
                            }
                        }
                    }
                    else
                    {
                        result.ok = false;
                        result.err = $"Property with name \"{expectedProp.Name}\" has other type (expected: {expectedProp.Value.ValueKind}, actaul: {responceElem.ValueKind})";
                        break;
                    }
                }
                else
                {
                    result.ok = false;
                    result.err = $"Responce hasn't property with name - \"{expectedProp.Name}\"";
                    break;
                }
            }
            return result;
        }

        private bool valueEquals(JsonElement elem1, JsonElement elem2)
        {
            var valueKind = elem1.ValueKind;
            
            if (valueKind == JsonValueKind.True || valueKind == JsonValueKind.False)
            {
                return elem1.GetBoolean() == elem2.GetBoolean();
            }
            else if (valueKind == JsonValueKind.Number)
            {
                return elem1.GetDecimal() == elem2.GetDecimal();
            }
            else if (valueKind == JsonValueKind.String)
            {
                return elem1.GetString() == elem2.GetString();
            }
            else if(valueKind == JsonValueKind.Null)
            {
                return true;
            }

            return false;
        }
    }
}
