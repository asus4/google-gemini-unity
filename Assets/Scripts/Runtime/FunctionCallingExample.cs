using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleApis.GenerativeLanguage;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace GoogleApis.Example
{
    /// <summary>
    /// Function Calling Example
    /// 
    /// https://ai.google.dev/docs/function_calling
    /// https://github.com/google-gemini/cookbook/blob/main/quickstarts/rest/Function_calling_REST.ipynb
    /// </summary>
    public sealed class FunctionCallingExample : MonoBehaviour
    {
        [Header("Scene references")]
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private TextMeshProUGUI messageLabel;

        [SerializeField]
        private Button sendButton;

        [SerializeField]
        [TextArea(1, 10)]
        private string systemInstruction = "You are a helpful assistant in Unity GameEngine. You can help users to create objects in the scene.";


        private GenerativeModel model;
        private readonly List<Content> messages = new();
        private static readonly StringBuilder sb = new();

        private Dictionary<int, GameObject> worldObjects = new();

        private Content systemInstructionContent;
        private Tool[] tools;

        private void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            model = client.GetModel(Models.Gemini_1_5_Pro);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "Make a big floor then make a cube on it.";

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                systemInstructionContent = new Content(new Content.Part[] { systemInstruction });
            }
            // Build Tools from all [FunctionCall("description")] attributes in the script.
            tools = new Tool[] { this.BuildFunctionsFromAttributes() };
            Debug.Log($"tools:\n{tools.First()}");


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
            messages.Add(content);
            RefreshView();

            // 1. Make request with Tools
            GenerateContentRequest request = new()
            {
                contents = messages,
                tools = tools,
            };
            if (systemInstructionContent != null)
            {
                request.systemInstruction = systemInstructionContent;
            }
            var response1 = await model.GenerateContentAsync(request, destroyCancellationToken);

            // 2. Receive function call response
            var modelContent = response1.candidates.First().content;
            messages.Add(modelContent);
            RefreshView();

            if (!modelContent.ContainsFunctionCall())
            {
                return;
            }

            // 3. Invoke function call in local client
            Content functionResponseContent = this.InvokeFunctionCalls(modelContent);

            // 4. Send function response back to model
            messages.Add(functionResponseContent);
            RefreshView();

            // 5. Generate content with function response
            var response2 = await model.GenerateContentAsync(request, destroyCancellationToken);
            messages.Add(response2.candidates.First().content);
            RefreshView();
        }

        private void RefreshView()
        {
            sb.Clear();
            foreach (var message in messages)
            {
                sb.AppendTMPRichText(message);
            }
            messageLabel.SetText(sb);
        }

        #region Function Calls

        [Preserve]
        [FunctionCall("Make a floor at the given scale then return the instance ID.")]
        public int MakeFloor(
            [FunctionCall("Scale of the floor")] Vector3 scale)
        {
            if (scale == Vector3.zero)
            {
                scale = Vector3.one;
            }
            return MakePrimitive(PrimitiveType.Plane, Vector3.zero, Vector3.zero, scale);
        }

        [Preserve]
        [FunctionCall("Make a cube at the given position, rotation, and scale then return the instance ID.")]
        public int MakeCube(
            [FunctionCall("Center position in the world space")] Vector3 position,
            [FunctionCall("Euler angles")] Vector3 rotation,
            [FunctionCall("Size")] Vector3 scale)
        {
            return MakePrimitive(PrimitiveType.Cube, position, rotation, scale);
        }

        [Preserve]
        [FunctionCall("Make a sphere at the given position and size then return the instance ID.")]
        public int MakeSphere(
            [FunctionCall("Center position in the world space")] Vector3 position,
            [FunctionCall("Size")] Vector3 size)
        {
            return MakePrimitive(PrimitiveType.Sphere, position, Vector3.zero, size);
        }

        private int MakePrimitive(PrimitiveType type, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            var go = GameObject.CreatePrimitive(type);
            go.transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
            go.transform.localScale = scale;

            // Add to the worldObjects
            int id = go.GetInstanceID();
            worldObjects.Add(id, go);
            return id;
        }

        #endregion Function Calls
    }
}
