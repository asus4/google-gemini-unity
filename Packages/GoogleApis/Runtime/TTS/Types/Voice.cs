namespace GoogleApis.TTS
{

    public record Voice
    {
        public string[] languageCodes;
        public string name;
        public SsmlVoiceGender ssmlGender;
        public int naturalSampleRateHertz;
    }

    public record VoicesResponse
    {
        public Voice[] voices;
    }


}
