#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gemini
{
    /// <summary>
    /// Extension methods for calling functions via reflection.
    /// </summary>
    public static class FunctionCallingExtensions
    {
        private const BindingFlags DefaultBindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        public static object? InvokeFunctionCall(this object obj, Content.FunctionCall functionCall, BindingFlags flags = DefaultBindings)
        {
            MethodInfo method = obj.GetType().GetMethod(functionCall.name, flags)
                ?? throw new MissingMethodException(obj.GetType().Name, functionCall.name);

            // No arguments
            if (functionCall.args == null)
            {
                return method.Invoke(obj, null);
            }

            // With arguments
            object[] parameters = new object[functionCall.args.Count];
            ParameterInfo[] methodParameters = method.GetParameters();
            for (int i = 0; i < methodParameters.Length; i++)
            {
                ParameterInfo parameter = methodParameters[i];
                if (!functionCall.args.TryGetValue(parameter.Name, out object value))
                {
                    throw new ArgumentException($"Missing argument: {parameter.Name}");
                }
                parameters[i] = value;
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
            BindingFlags flags = DefaultBindings)
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
            Type type = parameter.ParameterType;
            Tool.Type toolType = type.AsToolType();

            return new Tool.Schema()
            {
                type = toolType,
                format = type.GetTypeFormat(),
                description = parameter.GetCustomAttribute<FunctionCallAttribute>()?.description,
                nullable = parameter.IsOptional ? true : null,
                enums = type.IsEnum ? Enum.GetNames(type) : null,
                // TODO: implement nested object properties
                properties = toolType == Tool.Type.OBJECT ? type.GetProperties(BindingFlags.Public).ToDictionary(
                    property => property.Name,
                    property => property.PropertyType.ToSchema()
                ) : null,
                // required = new string[]{},
                items = toolType == Tool.Type.ARRAY ? type.GetElementType().ToSchema() : null
            };
        }

        private static Tool.Schema ToSchema(this Type type)
        {
            Tool.Type toolType = type.AsToolType();

            return new Tool.Schema()
            {
                type = toolType,
                format = type.GetTypeFormat(),
                enums = type.IsEnum ? Enum.GetNames(type) : null,
                properties = toolType == Tool.Type.OBJECT ? type.GetProperties(BindingFlags.Public).ToDictionary(
                    property => property.Name,
                    property => property.PropertyType.ToSchema()
                ) : null,
                items = toolType == Tool.Type.ARRAY ? type.GetElementType().ToSchema() : null
            };
        }

        private static Tool.Type AsToolType(this Type type)
        {
            return type switch
            {
                Type when type == typeof(string) => Tool.Type.STRING,
                Type when type == typeof(Enum) => Tool.Type.STRING,
                Type when type == typeof(float) => Tool.Type.NUMBER,
                Type when type == typeof(double) => Tool.Type.NUMBER,
                Type when type == typeof(int) => Tool.Type.INTEGER,
                Type when type == typeof(long) => Tool.Type.INTEGER,
                Type when type == typeof(bool) => Tool.Type.BOOLEAN,
                Type when type == typeof(object) => Tool.Type.OBJECT,
                Type when type == typeof(Array) => Tool.Type.ARRAY,
                Type when type == typeof(ICollection) => Tool.Type.ARRAY,
                _ => Tool.Type.OBJECT,
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

