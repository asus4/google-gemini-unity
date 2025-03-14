#nullable enable

using System;
using System.Text.Json.Serialization;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Configuration options for model generation and outputs. Not all parameters may be configurable for every model.
    ///
    /// https://ai.google.dev/api/generate-content#v1beta.GenerationConfig
    /// </summary>
    public record GenerationConfig
    {
        public const string DefaultResponseMimeType = "text/plain";
        public const string JSONResponseMimeType = "application/json";

        /// <summary>
        /// Optional. The set of character sequences (up to 5) that will stop output generation. If specified, the API will stop at the first appearance of a stop_sequence. The stop sequence will not be included as part of the response.
        /// </summary>
        [JsonPropertyName("stopSequences")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[]? StopSequences { get; set; }

        /// <summary>
        /// Optional. MIME type of the generated candidate text. Supported MIME types are: text/plain: (default) Text output. application/json: JSON response in the response candidates. text/x.enum: ENUM as a string response in the response candidates. Refer to the docs for a list of all supported text MIME types.
        /// </summary>
        [JsonPropertyName("responseMimeType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ResponseMimeType { get; set; }

        /// <summary>
        /// Optional. Output schema of the generated candidate text. Schemas must be a subset of the OpenAPI schema and can be objects, primitives or arrays.
        /// </summary>
        [JsonPropertyName("responseSchema")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Tool.Schema? ResponseSchema { get; set; }

        /// <summary>
        /// Optional. The requested modalities of the response. Represents the set of modalities that the model can return, and should be expected in the response. This is an exact match to the modalities of the response.
        /// A model may have multiple combinations of supported modalities. If the requested modalities do not match any of the supported combinations, an error will be returned.
        /// An empty list is equivalent to requesting only text.
        /// </summary>
        [JsonPropertyName("responseModalities")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Modality[]? ResponseModalities { get; set; }

        /// <summary>
        /// Optional. Number of generated responses to return. If unset, this will default to 1. Please note that this doesn't work for previous generation models (Gemini 1.0 family)
        /// </summary>
        [JsonPropertyName("candidateCount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CandidateCount { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to include in a response candidate.
        /// Note: The default value varies by model, see the Model.output_token_limit attribute of the Model returned from the getModel function.
        /// </summary>
        [JsonPropertyName("maxOutputTokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? MaxOutputTokens { get; set; }

        /// <summary>
        /// Optional. Controls the randomness of the output.
        /// Note: The default value varies by model, see the Model.temperature attribute of the Model returned from the getModel function.
        /// Values can range from [0.0, 2.0].
        /// </summary>
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double? Temperature { get; set; }

        /// <summary>
        /// Optional. The maximum cumulative probability of tokens to consider when sampling.
        /// The model uses combined Top-k and Top-p (nucleus) sampling.
        /// Tokens are sorted based on their assigned probabilities so that only the most likely tokens are considered. Top-k sampling directly limits the maximum number of tokens to consider, while Nucleus sampling limits the number of tokens based on the cumulative probability.
        /// Note: The default value varies by Model and is specified by theModel.top_p attribute returned from the getModel function. An empty topK attribute indicates that the model doesn't apply top-k sampling and doesn't allow setting topK on requests.
        /// </summary>
        [JsonPropertyName("topP")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double? TopP { get; set; }

        /// <summary>
        /// Optional. The maximum number of tokens to consider when sampling.
        /// Gemini models use Top-p (nucleus) sampling or a combination of Top-k and nucleus sampling. Top-k sampling considers the set of topK most probable tokens. Models running with nucleus sampling don't allow topK setting.
        /// Note: The default value varies by Model and is specified by theModel.top_p attribute returned from the getModel function. An empty topK attribute indicates that the model doesn't apply top-k sampling and doesn't allow setting topK on requests.
        /// </summary>
        [JsonPropertyName("topK")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? TopK { get; set; }

        /// <summary>
        /// Optional. Seed used in decoding. If not set, the request uses a randomly generated seed.
        /// </summary>
        [JsonPropertyName("seed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Seed { get; set; }

        /// <summary>
        /// Optional. Presence penalty applied to the next token's logprobs if the token has already been seen in the response.
        /// This penalty is binary on/off and not dependant on the number of times the token is used (after the first). Use frequencyPenalty for a penalty that increases with each use.
        /// A positive penalty will discourage the use of tokens that have already been used in the response, increasing the vocabulary.
        /// A negative penalty will encourage the use of tokens that have already been used in the response, decreasing the vocabulary.
        /// </summary>
        [JsonPropertyName("presencePenalty")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float? PresencePenalty { get; set; }

        /// <summary>
        /// Optional. Frequency penalty applied to the next token's logprobs, multiplied by the number of times each token has been seen in the respponse so far.
        /// A positive penalty will discourage the use of tokens that have already been used, proportional to the number of times the token has been used: The more a token is used, the more dificult it is for the model to use that token again increasing the vocabulary of responses.
        /// Caution: A negative penalty will encourage the model to reuse tokens proportional to the number of times the token has been used. Small negative values will reduce the vocabulary of a response. Larger negative values will cause the model to start repeating a common token until it hits the maxOutputTokens limit.
        /// </summary>
        [JsonPropertyName("frequencyPenalty")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float? FrequencyPenalty { get; set; }

        /// <summary>
        /// Optional. If true, export the logprobs results in response.
        /// </summary>
        [JsonPropertyName("responseLogprobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? ResponseLogprobs { get; set; }

        /// <summary>
        /// Optional. Only valid if responseLogprobs=True. This sets the number of top logprobs to return at each decoding step in the Candidate.logprobs_result.
        /// </summary>
        [JsonPropertyName("logprobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Logprobs { get; set; }

        /// <summary>
        /// Optional. Enables enhanced civic answers. It may not be available for all models.
        /// </summary>
        [JsonPropertyName("enableEnhancedCivicAnswers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? EnableEnhancedCivicAnswers { get; set; }

        // TODO: 
        // speechConfig
        // mediaResolution

        public void SetJsonMode(Type responseType)
        {
            ResponseMimeType = JSONResponseMimeType;
            ResponseSchema = responseType.ToSchema();
        }
    }
}
