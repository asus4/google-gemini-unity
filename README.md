# Google Gemini for Unity

**🚧 Work in progress 🏗️**

Non-official Google Gemini API client for Unity. Limited use cases are currently supported.

## How to use

**(🚧 TBD:)**  
Open examples scenes at `Assets/Scenes/*.scene

The following examples are available:

- BasicChatExample: Generates text content and streaming
- VisionExample: Image content understanding
- AudioExample: Audio content understanding
- FunctionCallingExample: Calling C# functions from Gemini
- TextToSpeechExample: Text-to-speech

## How to Run the example

1. [Enable API key at Google Cloud](https://console.cloud.google.com/apis/credentials)
2. Put `.env` file in the project root with the following content:

```sh
API_KEY=abc123
```

## How to install via UPM

Add the following line to `Packages/manifest.json` to install the Gemini for Unity package via UPM:

```json
"dependencies": {
    "com.github.asus4.googleapis-unity": "https://github.com/asus4/google-gemini-unity.git?path=Packages/GoogleApis",
    ... other dependencies
}
```

## Third-party assets and library licenses used in examples

- [Nunito Font](https://fonts.google.com/specimen/Nunito): OFL 1.1
- [Sawarabi Gothic Font](https://fonts.google.com/specimen/Sawarabi+Gothic): OFL 1.1
- [JFK library](https://www.jfklibrary.org/asset-viewer/archives/jfkwha-006): public domain
