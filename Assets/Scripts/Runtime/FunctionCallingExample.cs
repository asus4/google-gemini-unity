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
        public enum Model
        {
            Gemini_1_0_Pro,
            Gemini_1_5_Pro,
        }

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
        private Model modelType = Model.Gemini_1_0_Pro;

        [SerializeField]
        [TextArea(1, 10)]
        private string systemInstruction = "You are a helpful assistant in Unity GameEngine. You can help users to create objects in the scene.";


        private GenerativeModel model;
        private readonly List<Content> messages = new();
        private static readonly StringBuilder sb = new();

        private readonly Dictionary<int, GameObject> worldObjects = new();

        private Content systemInstructionContent;
        private Tool[] tools;

        private void Start()
        {
            using var settings = GoogleApiSettings.Get();
            var client = new GenerativeAIClient(settings);

            // Use 1.0 as 1.5 is rate limited in 5/minute, or increase the rate limit of 1.5
            string modelName = modelType switch
            {
                Model.Gemini_1_0_Pro => Models.GeminiPro,
                Model.Gemini_1_5_Pro => Models.Gemini_1_5_Pro,
                _ => throw new System.NotImplementedException(),
            };
            model = client.GetModel(modelName);

            // Setup UIs
            sendButton.onClick.AddListener(async () => await SendRequest());
            inputField.onSubmit.AddListener(async _ => await SendRequest());

            // for Debug
            inputField.text = "Make a big floor then make a cube on it.";

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                if (model.SupportsSystemInstruction)
                {
                    systemInstructionContent = new Content(new Content.Part[] { systemInstruction });
                }
                else
                {
                    // Add to user text if system instruction is not supported
                    inputField.text = $"{systemInstruction}\n---\n{inputField.text}";
                }
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

            while (true)
            {
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

                // 2. Receive response
                var response = await model.GenerateContentAsync(request, destroyCancellationToken);
                var modelContent = response.candidates.First().content;
                messages.Add(modelContent);
                RefreshView();

                // Stop if no function call in the response
                if (!modelContent.ContainsFunctionCall())
                {
                    return;
                }

                // 3. Invoke function call in local client
                Content functionResponseContent = this.InvokeFunctionCalls(modelContent);

                // 4. Send function response back to model
                messages.Add(functionResponseContent);
                RefreshView();
            }
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
        [FunctionCall("Make a floor at the given scale then returns the instance ID.")]
        public int MakeFloor(
            [FunctionCall("Scale of the floor")] float scale)
        {
            return MakePrimitive(PrimitiveType.Plane, Vector3.zero, Vector3.zero, Vector3.one * scale);
        }

        [Preserve]
        [FunctionCall("Make a cube at the given position, rotation, and scale then returns the instance ID.")]
        public int MakeCube(
            [FunctionCall("Center position in the world space")] Vector3 position,
            [FunctionCall("Euler angles")] Vector3 rotation,
            [FunctionCall("Size")] Vector3 scale)
        {
            return MakePrimitive(PrimitiveType.Cube, position, rotation, scale);
        }

        [Preserve]
        [FunctionCall("Make a sphere at the given position and size then returns the instance ID.")]
        public int MakeSphere(
            [FunctionCall("Center position in the world space")] Vector3 position,
            [FunctionCall("Scale of the sphere")] Vector3 scale)
        {
            return MakePrimitive(PrimitiveType.Sphere, position, Vector3.zero, scale);
        }

        [Preserve]
        [FunctionCall("Move the object to the given position.")]
        public void MoveObject(
            [FunctionCall("Instance ID of the object")] int id,
            [FunctionCall("New position in the world space")] Vector3 position)
        {
            if (worldObjects.TryGetValue(id, out GameObject go))
            {
                go.transform.position = position;
            }
        }

        [Preserve]
        [FunctionCall("Rotate the object to the given euler angles.")]
        public void RotateObject(
            [FunctionCall("Instance ID of the object")] int id,
            [FunctionCall("New euler angles")] Vector3 rotation)
        {
            if (worldObjects.TryGetValue(id, out GameObject go))
            {
                go.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        [Preserve]
        [FunctionCall("Scale the object to the given size.")]
        public void ScaleObject(
            [FunctionCall("Instance ID of the object")] int id,
            [FunctionCall("New size")] Vector3 scale)
        {
            if (worldObjects.TryGetValue(id, out GameObject go))
            {
                go.transform.localScale = scale;
            }
        }

        private int MakePrimitive(PrimitiveType type, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            // Gemini forgets setting scale sometimes
            if (scale == Vector3.zero)
            {
                scale = Vector3.one;
            }

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
