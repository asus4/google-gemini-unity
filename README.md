# Google Gemini API for Unity Example

**🚧 Work in progress 🏗️**
Non official Google Gemini API client for Unity

## How to Run the example

1. [Enable API key at Google AI Studio](https://aistudio.google.com/app/apikey)
2. Put `.env` file in the project root with the following content:

```sh
API_KEY=abc123
```

## How to install the Gemini for Unity library

Add the following line to `Packages/manifest.json` to install the Gemini for Unity package via UPM:

```json
"dependencies": {
    "com.github.asus4.google-gemini-unity": "https://github.com/asus4/google-gemini-unity.git?path=Packages/Gemini",
    ... other dependencies
}
```

## Third-party assets and libraries licenses in examples

- [Nunito Font](https://fonts.google.com/specimen/Nunito): OFL 1.1
- [Sawarabi Gothic Font](https://fonts.google.com/specimen/Sawarabi+Gothic): OFL 1.1
- [JFK library](https://www.jfklibrary.org/asset-viewer/archives/jfkwha-006): public domain
