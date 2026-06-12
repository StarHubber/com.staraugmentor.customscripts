using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
public static class UnityWebRequestExtensions
{
    public static Task AsTask(this UnityWebRequestAsyncOperation operation)
    {
        var tcs = new TaskCompletionSource<bool>();

        operation.completed += _ =>
        {
            tcs.SetResult(true);
        };

        return tcs.Task;
    }
}

public class MicrosoftTranslator
{
    private string subscriptionKey = "DEIN_AZURE_TRANSLATOR_KEY";
    private string region = "germanywestcentral"; // aus Azure: Keys and Endpoint
    private string endpoint = "https://api.cognitive.microsofttranslator.com";

    /*public IEnumerator Start()
    {
        yield return Translate("Fuel pump", "de", result =>
        {
            Debug.Log("Übersetzung: " + result);
        });
    }*/

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        string route = $"/translate?api-version=3.0&to={targetLanguage}";
        string url = endpoint + route;

        string jsonBody = "[{\"Text\":\"" + EscapeJson(text) + "\"}]";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.SetRequestHeader("Ocp-Apim-Subscription-Region", region);

        await request.SendWebRequest().AsTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.responseCode + " - " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }

        string response = request.downloadHandler.text;
        Debug.Log(response);

        string translatedText = ParseFirstTranslation(response);
        return translatedText;
    }

    private string ParseFirstTranslation(string json)
    {
        TranslatorResponse[] response = JsonHelper.FromJson<TranslatorResponse>(json);

        if (response.Length == 0 ||
            response[0].translations == null ||
            response[0].translations.Length == 0)
        {
            return "";
        }

        return response[0].translations[0].text;
    }

    private string EscapeJson(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}

[Serializable]
public class TranslatorResponse
{
    public Translation[] translations;
}

[Serializable]
public class Translation
{
    public string text;
    public string to;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}