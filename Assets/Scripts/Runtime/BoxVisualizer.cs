using System;
using System.Collections.Generic;
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

    public List<BoxData> boxesData = new() {
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
    public float fov = 60f;

    [SerializeField]
    private Texture2D _texture;

    static readonly Color LineColor = new(0x29 / 255f, 0x62 / 255f, 0xFF / 255f, 1);
    static readonly Lazy<GUIStyle> LabelStyle = new(()
    => new()
    {
        normal = new GUIStyleState { textColor = Color.white },
        fontSize = 12,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter
    });

    private void OnDrawGizmos()
    {
        if (_texture == null)
        {
            return;
        }

        DrawPerspectiveView();
    }

    private void DrawPerspectiveView()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // 画像の原点（左上）をシーンの原点に合わせる変換行列
        float imageWidth = _texture.width;
        float imageHeight = _texture.height;
        float aspectRatio = imageHeight / imageWidth;
        float scaleX = 0.1f / aspectRatio; // 5は任意。画面に収まるようなスケールにする
        float scaleY = 0.1f;  // 縦横比から計算する
        Vector3 imageCenterOffset = new Vector3(scaleX / 2f, -scaleY / 2f, 0);

        // Gizmosの位置、回転、スケールを設定
        Gizmos.matrix = Matrix4x4.TRS(transform.position - imageCenterOffset, Quaternion.identity, new Vector3(scaleX, scaleY, 1));
        Gizmos.DrawGUITexture(new Rect(0, 0, 1, 1), _texture);
        Gizmos.matrix = originalMatrix;


        // Calculate camera intrinsics
        float f = (scaleX * imageWidth) / (2 * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad));
        Vector3 cameraCenter = transform.position; // Use the GameObject's position as the camera position


        foreach (BoxData boxData in boxesData)
        {
            DrawBox(boxData, f, cameraCenter, scaleX, scaleY);
        }

    }


    private void DrawBox(BoxData boxData, float focalLength, Vector3 cameraCenter, float scaleX, float scaleY)
    {
        Vector3 center = boxData.center;
        Vector3 size = boxData.size;
        Quaternion rotation = Quaternion.Euler(boxData.rotation);

        // Calculate half size for convenience
        Vector3 halfSize = size * 0.5f;

        // Define the 8 corners of the box in local space
        Vector3[] corners = new Vector3[]
        {
            new (-halfSize.x, -halfSize.y, -halfSize.z),
            new ( halfSize.x, -halfSize.y, -halfSize.z),
            new (-halfSize.x,  halfSize.y, -halfSize.z),
            new ( halfSize.x,  halfSize.y, -halfSize.z),
            new (-halfSize.x, -halfSize.y,  halfSize.z),
            new ( halfSize.x, -halfSize.y,  halfSize.z),
            new (-halfSize.x,  halfSize.y,  halfSize.z),
            new ( halfSize.x,  halfSize.y,  halfSize.z),
        };

        // Transform corners to world space
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * corners[i] + center;
        }

        // Reorder corners to match the original JavaScript code
        Vector3[] reorderedCorners = new Vector3[] {
            corners[1], corners[3], corners[7], corners[5],
            corners[0], corners[2], corners[6], corners[4]
        };
        corners = reorderedCorners;

        // Create the view rotation matrix (90-degree tilt)
        Matrix4x4 viewRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));

        // Rotate, translate points, and calculate distances
        List<Vector3> rotatedPoints = new List<Vector3>();
        List<float> vertexDistances = new List<float>();

        foreach (var p in corners)
        {
            Vector3 rotatedPoint = viewRotationMatrix.MultiplyPoint(p);
            rotatedPoints.Add(rotatedPoint);
            vertexDistances.Add(rotatedPoint.magnitude);
        }

        // Project points
        List<Vector2> projectedPoints = new List<Vector2>();
        foreach (var p in rotatedPoints)
        {
            float projectedX = (focalLength * p.x / p.z);
            float projectedY = (focalLength * p.y / p.z);

            // Scale and center projected points to image coordinates
            projectedX += scaleX * 0.5f; // シーンの原点
            projectedY = -projectedY + (scaleY * 0.5f);  // UnityのY軸は下向き

            projectedPoints.Add(new Vector2(projectedX, projectedY));
        }

        Gizmos.color = LineColor;

        DrawProjectedLines(projectedPoints);


        // Project text position
        Vector3 rotatedTextPoint = viewRotationMatrix.MultiplyPoint(center);
        float textProjectedX = focalLength * rotatedTextPoint.x / rotatedTextPoint.z;
        float textProjectedY = focalLength * rotatedTextPoint.y / rotatedTextPoint.z;

        textProjectedX += scaleX * 0.5f;
        textProjectedY = -textProjectedY + (scaleY * 0.5f);

        // Draw the label
        Handles.Label(
            new Vector3(textProjectedX, textProjectedY, 0),
            boxData.label,
            LabelStyle.Value);
    }

    static void DrawProjectedLines(List<Vector2> projectedPoints)
    {
        // Split vertices into top and bottom
        List<Vector2> topVertices = projectedPoints.GetRange(0, 4);
        List<Vector2> bottomVertices = projectedPoints.GetRange(4, 4);

        // Draw lines between the vertices
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
