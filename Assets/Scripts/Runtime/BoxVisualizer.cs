using System;
using UnityEngine;
using UnityEditor;

public class BoxVisualizer : MonoBehaviour
{
    [Serializable]
    public class BoxData
    {
        public string label;
        public Vector3 center;
        public Vector3 size;
        public Vector3 rotation; // Euler angles (degrees)
    }

    [SerializeField]
    BoxData[] boxesData = new BoxData[]
    {
        new () { label = "range hood", center = new Vector3(-0.11f, 3.3f, 0.74f), size = new Vector3(0.8f, 0.45f, 0.57f), rotation = new Vector3(4, 0, 0) },
        new () { label = "range", center = new Vector3(-0.11f, 3.26f, -0.67f), size = new Vector3(0.76f, 0.6f, 1.19f), rotation = new Vector3(4, 0, 0) },
        new () { label = "dishwasher", center = new Vector3(-0.88f, 2.31f, -0.86f), size = new Vector3(0.17f, 0.61f, 0.86f), rotation = new Vector3(4, 0, 0) },
        new () { label = "microwave", center = new Vector3(1.13f, 3.28f, -0.22f), size = new Vector3(0.49f, 0.32f, 0.29f), rotation = new Vector3(4, 0, 0) },
        new () { label = "coffee maker", center = new Vector3(0.63f, 3.29f, -0.14f), size = new Vector3(0.2f, 0.24f, 0.33f), rotation = new Vector3(4, 0, 0) },
        new () { label = "toaster", center = new Vector3(-0.99f, 3.31f, -0.16f), size = new Vector3(0.29f, 0.17f, 0.19f), rotation = new Vector3(4, 0, 0) },
        new () { label = "kitchen cabinets", center = new Vector3(-0.87f, 2.27f, -0.86f), size = new Vector3(0.47f, 1.86f, 0.97f), rotation = new Vector3(4, 0, 0) },
        new () { label = "kitchen cabinets", center = new Vector3(0.88f, 2.15f, 1.06f), size = new Vector3(0.47f, 1.86f, 0.97f), rotation = new Vector3(4, 0, 0) }
    };

    [Range(50, 120)]
    float fov = 60f;

    [SerializeField]
    Texture2D _texture;

    [SerializeField]
    [Range(0.001f, 0.02f)]
    float scale = 0.005f;

    [SerializeField]
    RectTransform imageContainer;

    static readonly Color LineColor = new(0x29 / 255f, 0x62 / 255f, 0xFF / 255f, 1);
    static readonly Lazy<GUIStyle> LabelStyle = new(()
    => new()
    {
        normal = new GUIStyleState { textColor = Color.white },
        fontSize = 10,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter
    });

    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (!enabled || _texture == null)
        {
            return;
        }

        DrawPerspectiveView();
    }

    private void DrawPerspectiveView()
    {
        // 画像の原点（左上）をシーンの原点に合わせる変換行列
        float aspectRatio = _texture.width / _texture.height;
        Vector3 worldCenter = imageContainer.position;
        Vector2 worldScale = new(scale / aspectRatio, scale);

        // Calculate camera intrinsics
        float f = (worldScale.x * _texture.width) / (2 * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad));

        Gizmos.color = LineColor;

        foreach (BoxData boxData in boxesData)
        {
            DrawBox(boxData, f, worldCenter, worldScale);
        }
    }

    static void DrawBox(BoxData boxData, float focalLength, Vector3 worldCenter, Vector2 scale)
    {
        Vector3 center = boxData.center;
        Quaternion rotation = Quaternion.Euler(boxData.rotation);

        // Calculate half size for convenience
        Vector3 halfSize = boxData.size * 0.5f;

        // Define the 8 corners of the box in local space
        Span<Vector3> corners = stackalloc Vector3[]
        {
            new ( halfSize.x, -halfSize.y, -halfSize.z), // 1
            new ( halfSize.x,  halfSize.y, -halfSize.z), // 3
            new ( halfSize.x,  halfSize.y,  halfSize.z),  // 7
            new ( halfSize.x, -halfSize.y,  halfSize.z),  // 5
            new (-halfSize.x, -halfSize.y, -halfSize.z), // 0
            new (-halfSize.x,  halfSize.y, -halfSize.z), // 2
            new (-halfSize.x,  halfSize.y,  halfSize.z),  // 6
            new (-halfSize.x, -halfSize.y,  halfSize.z), // 4
            Vector3.zero // Center
        };

        // Transform corners to world space
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * corners[i] + center;
        }

        // Create the view rotation matrix (90-degree tilt)
        Matrix4x4 viewRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));

        // Rotate, translate points, and calculate distances
        Span<Vector3> rotatedPoints = stackalloc Vector3[corners.Length];

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 rotatedPoint = viewRotationMatrix.MultiplyPoint(corners[i]);
            rotatedPoints[i] = rotatedPoint;
        }

        // Project points
        Span<Vector3> projectedPoints = stackalloc Vector3[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            float projectedX = focalLength * rotatedPoints[i].x / rotatedPoints[i].z;
            float projectedY = focalLength * rotatedPoints[i].y / rotatedPoints[i].z;

            // Scale and center projected points to image coordinates
            projectedX += scale.x * 0.5f; // シーンの原点
            projectedY = -projectedY + (scale.y * 0.5f);  // UnityのY軸は下向き

            projectedPoints[i] = new Vector3(projectedX, projectedY, 0) + worldCenter;
        }

        DrawProjectedLines(projectedPoints[..^1]);

        // Draw the label at center
        Handles.Label(projectedPoints[^1], boxData.label, LabelStyle.Value);
    }

    static void DrawProjectedLines(Span<Vector3> projectedPoints)
    {
        // Split vertices into top and bottom
        var topVertices = projectedPoints[..4];
        var bottomVertices = projectedPoints[4..];

        for (int i = 0; i < 4; i++)
        {
            // Top lines
            Gizmos.DrawLine(topVertices[i], topVertices[(i + 1) % 4]);
            // Bottom lines
            Gizmos.DrawLine(bottomVertices[i], bottomVertices[(i + 1) % 4]);
            // Connecting lines
            Gizmos.DrawLine(topVertices[i], bottomVertices[i]);
        }
    }
}
