using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GenerativeAI
{
    /// <summary>
    /// Basic Chat Examples
    /// </summary>
    public sealed class BasicChatExample : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI historyLabel;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        private bool showAvailableModels;

        private GenerativeModel model;

        private List<Content> messages = new();
        private readonly StringBuilder sb = new();

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;

            // Test MarkdownToTMPRichText
            string markdown = "## C#でStreaming APIを実装する方法\n\nC#でStreaming APIを実装する方法はいくつかありますが、ここでは一般的なアプローチとライブラリを紹介します。\n\n**1. ライブラリの選択**\n\n- **TweetinviAPI**: .NET向けのTwitter APIラッパーライブラリで、Streaming APIを含め、様々な機能を提供します。使いやすく、ドキュメントも充実しています。\n- **LINQ to Twitter**: LINQを使用してTwitter APIにアクセスできるライブラリです。Streaming APIにも対応しており、クエリ構文でストリームを処理できます。\n- **Twitterizer**: .NET Framework向けのTwitter APIラッパーライブラリです。Streaming APIにも対応しており、シンプルなAPIを提供します。\n\n**2. 認証**\n\nどのライブラリを使用する場合でも、Twitter APIを使用するには認証が必要です。開発者ポータルからAPIキーとアクセストークンを取得し、ライブラリに設定します。\n\n**3. ストリームの作成**\n\nライブラリごとに異なる方法でストリームを作成します。一般的には、以下の情報を指定します。\n\n- ストリームの種類（例: 公開ツイート、ユーザーのタイムライン、フィルターなど）\n- フィルター条件（キーワード、ユーザーIDなど）\n\n**4. ストリームの処理**\n\nストリームから取得したデータは、イベントハンドラーやコールバック関数で処理します。ツイートの内容、ユーザー情報、その他のメタデータにアクセスできます。\n\n**5. 例 (TweetinviAPIを使用)**\n\n```csharp\n// TweetinviAPIをインストール\nInstall-Package TweetinviAPI\n\n// 認証\nAuth.SetUserCredentials(\"consumer_key\", \"consumer_secret\", \"access_token\", \"access_token_secret\");\n\n// ストリームの作成 (公開ツイート)\nvar stream = Stream.CreateSampleStream();\n\n// イベントハンドラーの設定\nstream.TweetReceived += (sender, args) =\u003e\n{\n    Console.WriteLine(args.Tweet.Text);\n};\n\n// ストリームの開始\nstream.StartStream();\n```\n\n**その他のポイント**\n\n- エラー処理: ストリーム中にエラーが発生する可能性があるため、適切に処理する必要があります。\n- 再接続: ストリームが切断された場合は、自動的に再接続を試みるように実装することが推奨されます。\n- レート制限: Twitter APIにはレート制限があるため、制限を超えないように注意が必要です。\n\n**参考資料**\n\n- TweetinviAPI: https://github.com/linvi/tweetinvi\n- LINQ to Twitter: https://github.com/JoeMayo/LinqToTwitter\n- Twitterizer: https://github.com/Twitterizer/Twitterizer\n\n**注意:** 上記の情報は2023年11月時点のものです。APIの仕様やライブラリのバージョンによっては、変更されている可能性があります。最新の情報は、各ライブラリのドキュメントを参照してください。 \n";
            historyLabel.SetText(markdown.MarkdownToRichText());
        }

        private async void Start()
        {
            using var settings = GenerativeAISettings.Get();
            var client = new GenerativeAIClient(settings);

            // List all available models
            if (showAvailableModels)
            {
                var models = await client.ListModelsAsync(destroyCancellationToken);
                Debug.Log($"Available models: {models}");
            }

            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "Hello! what is your name?";
        }

        private async Task SendRequest()
        {
            var input = inputField.text;
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            inputField.text = string.Empty;

            Content content = new(Role.User, input);
            AppendToView(content);
            messages.Add(content);

            var response = await model.GenerateContentAsync(messages, destroyCancellationToken);
            Debug.Log($"Response: {response}");

            if (response.candidates.Length > 0)
            {
                var modelContent = response.candidates[0].content;
                AppendToView(modelContent);
                messages.Add(modelContent);
            }
        }

        private void AppendToView(Content content)
        {
            sb.AppendLine($"<b>{content.role}:</b>");
            foreach (var part in content.parts)
            {
                if (part.text != null)
                {
                    sb.AppendLine(part.text);
                }
                else
                {
                    sb.AppendLine($"<color=red>Unsupported part</color>");
                }
            }
            historyLabel.SetText(sb);
        }
    }
}
