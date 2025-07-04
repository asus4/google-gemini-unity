# Google Gemini for Unity

![imagen-example](https://github.com/user-attachments/assets/4c4dae15-c9a3-4fdb-a77a-0b31697d7c3f)

https://github.com/user-attachments/assets/304172b4-5b39-4ddf-b417-e65108e97da8


Non-official Google Gemini API client for Unity. Limited use cases are currently supported.

## How to use

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
GOOGLE_API_KEY=abc123
```

## How to install via UPM

1. This library depends on System.Text.Json. Add the following lines to `Packages/manifest.json` to install the Gemini for Unity package via UPM:

```json
"scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.cysharp.unitask",
        "com.github-glitchenzo.nugetforunity"
      ]
    }
],
"dependencies": {
    "com.github-glitchenzo.nugetforunity": "4.3.0",
}
```

2. From the Menu, select `NuGet` -> `Manage NuGet Packages`. Find `System.Text.Json` and install it.
![nuget-fig](https://github.com/user-attachments/assets/9dd9afdc-4230-4fda-851a-cbf6518f6c08)
3. Install this package via UPM.

```json
"dependencies": {
    "com.github.asus4.google-gemini": "https://github.com/asus4/google-gemini-unity.git?path=Packages/com.github.asus4.google-gemini#v0.2.8",
    ... other dependencies
}
```

## Third-party assets and library licenses used in examples

- [UniTask](https://github.com/Cysharp/UniTask): MIT
- [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity): MIT
- [Nunito Font](https://fonts.google.com/specimen/Nunito): OFL 1.1
- [Sawarabi Gothic Font](https://fonts.google.com/specimen/Sawarabi+Gothic): OFL 1.1
- [JFK library](https://www.jfklibrary.org/asset-viewer/archives/jfkwha-006): public domain
