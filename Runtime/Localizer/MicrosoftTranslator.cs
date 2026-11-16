using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
    private const int MaxCharactersPerRequest = 50000;
    private const int MaxTextsPerRequest = 100;

    private string subscriptionKey = "FgLdkaw4SQ7oR4KliELrkjTLzOc8I2MrJNBLQypAaxBsGaN46ElHJQQJ99CKACPV0roXJ3w3AAAbACOG7gU6";
    private string region = "germanywestcentral"; // aus Azure: Keys and Endpoint
    private string endpoint = "https://api.cognitive.microsofttranslator.com/";

    /*public IEnumerator Start()
    {
        yield return Translate("Fuel pump", "de", result =>
        {
            Debug.Log("�bersetzung: " + result);
        });
    }*/

    public async Task<string> TranslateAsync(string text, string targetLanguage)
    {
        string[][] translations = await TranslateBatchAsync(new[] { text }, new[] { targetLanguage });
        return translations[0][0];
    }

    public async Task<string[][]> TranslateBatchAsync(string[] texts, string[] targetLanguages)
    {
        if (texts == null)
            throw new ArgumentNullException(nameof(texts));
        if (targetLanguages == null || targetLanguages.Length == 0)
            throw new ArgumentException("At least one target language is required.", nameof(targetLanguages));

        string[][] translations = new string[texts.Length][];
        for (int i = 0; i < translations.Length; i++)
            translations[i] = new string[targetLanguages.Length];

        int textIndex = 0;
        while (textIndex < texts.Length)
        {
            List<string> batch = new List<string>();
            int batchCharacterCount = 0;

            while (textIndex < texts.Length && batch.Count < MaxTextsPerRequest)
            {
                string text = texts[textIndex] ?? string.Empty;
                long textCharacterCount = (long)text.Length * targetLanguages.Length;
                if (textCharacterCount > MaxCharactersPerRequest)
                {
                    throw new ArgumentException(
                        $"Text at index {textIndex} exceeds the {MaxCharactersPerRequest}-character request limit across target languages.",
                        nameof(texts));
                }

                if (batch.Count > 0 && batchCharacterCount + textCharacterCount > MaxCharactersPerRequest)
                    break;

                batch.Add(text);
                batchCharacterCount += (int)textCharacterCount;
                textIndex++;
            }

            string[][] batchTranslations = await TranslateChunkAsync(batch, targetLanguages);
            for (int i = 0; i < batchTranslations.Length; i++)
                translations[textIndex - batchTranslations.Length + i] = batchTranslations[i];
        }

        return translations;
    }

    private async Task<string[][]> TranslateChunkAsync(List<string> texts, string[] targetLanguages)
    {
        StringBuilder urlBuilder = new StringBuilder(endpoint).Append("translate?api-version=3.0");
        foreach (string targetLanguage in targetLanguages)
            urlBuilder.Append("&to=").Append(UnityWebRequest.EscapeURL(targetLanguage));

        StringBuilder jsonBuilder = new StringBuilder("[");
        for (int i = 0; i < texts.Count; i++)
        {
            if (i > 0)
                jsonBuilder.Append(',');
            jsonBuilder.Append("{\"Text\":\"").Append(EscapeJson(texts[i])).Append("\"}");
        }
        jsonBuilder.Append(']');

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBuilder.ToString());

        using UnityWebRequest request = new UnityWebRequest(urlBuilder.ToString(), "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.SetRequestHeader("Ocp-Apim-Subscription-Region", region);

        await request.SendWebRequest().AsTask();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new InvalidOperationException(
                $"Microsoft Translator request failed ({request.responseCode}): {request.error}. {request.downloadHandler.text}");
        }

        TranslatorResponse[] response = JsonHelper.FromJson<TranslatorResponse>(request.downloadHandler.text);
        if (response.Length != texts.Count)
            throw new InvalidOperationException("Microsoft Translator returned an unexpected number of translations.");

        string[][] translations = new string[texts.Count][];
        for (int textIndex = 0; textIndex < response.Length; textIndex++)
        {
            translations[textIndex] = new string[targetLanguages.Length];
            for (int languageIndex = 0; languageIndex < targetLanguages.Length; languageIndex++)
            {
                if (response[textIndex].translations == null)
                    continue;

                foreach (Translation translation in response[textIndex].translations)
                {
                    if (string.Equals(translation.to, targetLanguages[languageIndex], StringComparison.OrdinalIgnoreCase))
                    {
                        translations[textIndex][languageIndex] = translation.text;
                        break;
                    }
                }
            }
        }

        return translations;
    }

    private string EscapeJson(string value)
    {
        StringBuilder escaped = new StringBuilder(value.Length);
        foreach (char character in value)
        {
            switch (character)
            {
                case '\\': escaped.Append("\\\\"); break;
                case '"': escaped.Append("\\\""); break;
                case '\b': escaped.Append("\\b"); break;
                case '\f': escaped.Append("\\f"); break;
                case '\n': escaped.Append("\\n"); break;
                case '\r': escaped.Append("\\r"); break;
                case '\t': escaped.Append("\\t"); break;
                default:
                    if (char.IsControl(character))
                        escaped.Append("\\u").Append(((int)character).ToString("x4"));
                    else
                        escaped.Append(character);
                    break;
            }
        }

        return escaped.ToString();
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