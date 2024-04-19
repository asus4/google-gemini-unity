#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Extension methods for calling functions via reflection.
    /// </summary>
    public static class FunctionCallingExtensions
    {
        private const BindingFlags BindingsForCall = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        private const BindingFlags BindingsForTool = BindingFlags.Public | BindingFlags.Instance;

        public static object? InvokeFunctionCall(this object obj, Content.FunctionCall functionCall, BindingFlags flags = BindingsForCall)
        {
            MethodInfo method = obj.GetType().GetMethod(functionCall.name, flags)
                ?? throw new MissingMethodException(obj.GetType().Name, functionCall.name);

            // No arguments
            if (functionCall.args == null)
            {
                return method.Invoke(obj, null);
            }

            // With arguments
            ParameterInfo[] methodParameters = method.GetParameters();
            object?[] parameters = new object[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; i++)
            {
                ParameterInfo parameter = methodParameters[i];
                Type type = parameter.ParameterType;

                if (functionCall.args.TryGetValue(parameter.Name, out object value))
                {
                    parameters[i] = value.JsonCastTo(type);
                    Debug.Log($"Parameter: {parameter.Name}, Type: {type}, Value: {parameters[i]}");
                }
                else if (parameter.HasDefaultValue)
                {
                    parameters[i] = parameter.DefaultValue;
                }
                else
                {
                    parameters[i] = type.IsValueType ? Activator.CreateInstance(type) : null;
                }
            }
            return method.Invoke(obj, parameters);
        }

        public static Content.FunctionCall? FindFunctionCall(this Content content)
        {
            if (content.parts == null || content.parts.Count == 0)
            {
                return null;
            }
            if (content.parts.First().functionCall is Content.FunctionCall call)
            {
                return call;
            }
            return null;
        }

        // TODO: Consider migrating to source generator
        public static Tool.FunctionDeclaration[] BuildFunctionsFromAttributes(
            this object obj,
            BindingFlags flags = BindingsForTool)
        {
            var methods = obj.GetType().GetMethods(flags)
                .Where(method => method.GetCustomAttribute<FunctionCallAttribute>() != null);
            if (methods.Count() == 0)
            {
                throw new ArgumentException($"No methods with [FunctionCall] attribute found in {obj.GetType().Name}");
            }

            var list = new List<Tool.FunctionDeclaration>(methods.Count());
            foreach (var method in methods)
            {
                list.Add(MakeFunctionDeclaration(method));
            }
            return list.ToArray();
        }

        public static Tool.FunctionDeclaration MakeFunctionDeclaration(MethodInfo method)
        {
            string description = method.GetCustomAttribute<FunctionCallAttribute>()?.description
                ?? throw new ArgumentException($"Missing [FunctionCall(description)] on {method.Name}");
            Type returnType = method.ReturnType;
            ParameterInfo[]? parameters = method.GetParameters();

            return new Tool.FunctionDeclaration(
                name: method.Name,
                description: description,
                parameters: new Tool.Schema()
                {
                    type = returnType.AsToolType(),
                    format = returnType.GetTypeFormat(),
                    properties = parameters?.ToDictionary(
                        parameter => parameter.Name,
                        parameter => parameter.ToSchema()
                    ),
                }
            );
        }

        private static Tool.Schema ToSchema(this ParameterInfo parameter)
        {
            var schema = parameter.ParameterType.ToSchema(0);
            schema.description = parameter.GetCustomAttribute<FunctionCallAttribute>()?.description;
            schema.nullable = parameter.IsOptional;
            return schema;
        }

        private static Tool.Schema ToSchema(this Type type, int depth)
        {
            if (depth > 10)
            {
                throw new ArgumentException("Depth limit reached");
            }
            Tool.Type toolType = type.AsToolType();
            // Debug.Log($"Type: {type}, toolType: {toolType}, depth: {depth}");

            return new Tool.Schema()
            {
                type = toolType,
                format = type.GetTypeFormat(),
                nullable = false,
                enums = type.IsEnum ? Enum.GetNames(type) : null,
                properties = toolType == Tool.Type.OBJECT ? type.GetFields(BindingsForTool).ToDictionary(
                    field => field.Name,
                    field => field.FieldType.ToSchema(depth + 1)
                ) : null,
                items = toolType == Tool.Type.ARRAY ? type.GetElementType().ToSchema(depth + 1) : null
            };
        }

        private static Tool.Type AsToolType(this Type type)
        {
            return type switch
            {
                // String
                Type when type == typeof(string) => Tool.Type.STRING,
                Type when type == typeof(Enum) => Tool.Type.STRING,
                Type when type.IsEnum => Tool.Type.STRING,
                // Number
                Type when type == typeof(float) => Tool.Type.NUMBER,
                Type when type == typeof(double) => Tool.Type.NUMBER,
                // Integer
                Type when type == typeof(int) => Tool.Type.INTEGER,
                Type when type == typeof(long) => Tool.Type.INTEGER,
                // Boolean
                Type when type == typeof(bool) => Tool.Type.BOOLEAN,
                // Array
                Type when type.IsArray => Tool.Type.ARRAY,
                Type when type == typeof(Array) => Tool.Type.ARRAY,
                Type when type == typeof(ICollection) => Tool.Type.ARRAY,
                // Object
                Type when type == typeof(object) => Tool.Type.OBJECT,
                _ => Tool.Type.OBJECT, // and all other types
            };
        }

        private static string? GetTypeFormat(this Type type)
        {
            return type switch
            {
                Type when type == typeof(float) => "float",
                Type when type == typeof(double) => "double",
                Type when type == typeof(int) => "int32",
                Type when type == typeof(long) => "int64",
                _ => null,
            };
        }


    }
}

