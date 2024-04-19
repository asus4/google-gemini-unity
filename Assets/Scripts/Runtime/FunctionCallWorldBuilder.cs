using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Scripting;

namespace GoogleApis.Example
{
    [Preserve]
    public sealed class FunctionCallWorldBuilder : MonoBehaviour
    {
        private readonly Dictionary<int, GameObject> worldObjects = new();

        [Description("Make a floor at the given scale then returns the instance ID.")]
        public int MakeFloor(
            [Description("Scale of the floor")] float scale)
        {
            return MakePrimitive(PrimitiveType.Plane, Vector3.zero, Vector3.zero, Vector3.one * scale);
        }

        [Description("Make a cube at the given position, rotation, and scale then returns the instance ID.")]
        public int MakeCube(
            [Description("Center position in the world space")] Vector3 position,
            [Description("Euler angles")] Vector3 rotation,
            [Description("Size")] Vector3 scale)
        {
            return MakePrimitive(PrimitiveType.Cube, position, rotation, scale);
        }

        [Description("Make a sphere at the given position and size then returns the instance ID.")]
        public int MakeSphere(
            [Description("Center position in the world space")] Vector3 position,
            [Description("Scale of the sphere")] Vector3 scale)
        {
            return MakePrimitive(PrimitiveType.Sphere, position, Vector3.zero, scale);
        }

        [Description("Move the object to the given position.")]
        public void MoveObject(
            [Description("Instance ID of the object")] int id,
            [Description("New position in the world space")] Vector3 position)
        {
            if (worldObjects.TryGetValue(id, out GameObject go))
            {
                go.transform.position = position;
            }
        }

        [Description("Rotate the object to the given euler angles.")]
        public void RotateObject(
            [Description("Instance ID of the object")] int id,
            [Description("New euler angles")] Vector3 rotation)
        {
            if (worldObjects.TryGetValue(id, out GameObject go))
            {
                go.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        [Description("Scale the object to the given size.")]
        public void ScaleObject(
            [Description("Instance ID of the object")] int id,
            [Description("New size")] Vector3 scale)
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
    }
}
