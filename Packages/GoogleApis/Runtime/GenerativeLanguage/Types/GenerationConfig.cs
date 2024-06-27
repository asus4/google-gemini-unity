#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Configuration options for model generation and outputs. Not all parameters may be configurable for every model.
    ///
    /// https://ai.google.dev/api/rest/v1beta/GenerationConfig
    /// </summary>
    public record GenerationConfig
    {
        public const string DefaultResponseMimeType = "text/plain";
        public const string JSONResponseMimeType = "application/json";

        public ICollection<string>? stopSequences;
        public string? responseMimeType;
        public Tool.Schema? responseSchema;
        public int? candidateCount;
        public int? maxOutputTokens;
        [Range(0, 2)]
        public double? temperature;
        public double? topP;
        public int? topK;

        public void SetJsonMode(Type responseType)
        {
            responseMimeType = JSONResponseMimeType;
            responseSchema = responseType.ToSchema();
        }
    }
}
