namespace GoogleApis.TTS
{
    /// <summary>
    /// https://cloud.google.com/text-to-speech/docs/reference/rest/v1beta1/SsmlVoiceGender
    /// </summary>
    public enum SsmlVoiceGender
    {
        SSML_VOICE_GENDER_UNSPECIFIED,//   An unspecified gender. In VoiceSelectionParams, this means that the client doesn't care which gender the selected voice will have. In the Voice field of ListVoicesResponse, this may mean that the voice doesn't fit any of the other categories in this enum, or that the gender of the voice isn't known.
        MALE, // A male voice.
        FEMALE, // A female voice.
        NEUTRAL, // A gender-neutral voice. This voice is not yet supported.
    }
}
