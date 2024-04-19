#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Extension methods for calling functions via reflection.
    /// </summary>
    public static class FunctionCallingExtensions
    {
        private const string NO_DESCRIPTION_ATTRIBUTE = "Add [System.ComponentModel.Description(description)] to use function calling";
        private const BindingFlags DefaultBindings = BindingFlags.Public | BindingFlags.Instance;

        public static async Task<object?> InvokeFunctionCallAsync(
            this object instance,
            Content.FunctionCall functionCall,
            CancellationToken cancellationToken = default,
            BindingFlags flags = DefaultBindings)
        {
            MethodInfo method = instance.GetType().GetMethod(functionCall.name, flags)
                ?? throw new MissingMethodException(instance.GetType().Name, functionCall.name);

            // No arguments
            if (functionCall.args == null)
            {
                return method.Invoke(instance, null);
            }

            // Build method parameter arguments
            ParameterInfo[] methodParameters = method.GetParameters();
            object?[] parameters = new object[methodParameters.Length];
            for (int i = 0; i < methodParameters.Length; i++)
            {
                ParameterInfo parameter = methodParameters[i];
                Type type = parameter.ParameterType;

                if (type == typeof(CancellationToken))
                {
                    parameters[i] = cancellationToken;
                    continue;
                }
                if (functionCall.args.TryGetValue(parameter.Name, out object value))
                {
                    parameters[i] = value.JsonCastTo(type);
                    // Debug.Log($"Parameter: {parameter.Name}, Type: {type}, Value: {parameters[i]}");
                    continue;
                }
                if (parameter.HasDefaultValue)
                {
                    parameters[i] = parameter.DefaultValue;
                    continue;
                }
                // else 
                parameters[i] = type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            // Return inside type(T) of Task<T> in case of async method
            Type returnType = method.ReturnType;
            bool isGenericTask = returnType.IsGenericTask();
            bool isTask = returnType == typeof(Task);
            if (isTask || isGenericTask)
            {
                Task task = (Task)method.Invoke(instance, parameters);
                await task;
                cancellationToken.ThrowIfCancellationRequested();
                if (isTask)
                {
                    return null;
                }
                return task.GetType().GetProperty("Result").GetValue(task, null);
            }
            else
            {
                return method.Invoke(instance, parameters);
            }
        }

        public static async Task<Content> InvokeFunctionCallsAsync(
            this object instance,
            Content functionCallContent,
            CancellationToken cancellationToken = default,
            BindingFlags flags = DefaultBindings)
        {
            if (!functionCallContent.ContainsFunctionCall())
            {
                throw new ArgumentException("No function calls found in content");
            }

            List<Content.Part> parts = new();
            foreach (var part in functionCallContent.parts)
            {
                if (part.functionCall == null)
                {
                    continue;
                }
                string name = part.functionCall.name;
                try
                {
                    object? result = await instance.InvokeFunctionCallAsync(
                        part.functionCall,
                        cancellationToken,
                        flags);
                    parts.Add(new Content.FunctionResponse(name, result));
                }
                catch (Exception e)
                {
                    parts.Add(new Content.FunctionResponse(name, new(e.ToString(), e.Message)));
                    Debug.LogError($"Error invoking function {name}: {e.Message}");
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
            return new Content(Role.Function, parts);
        }

        /// <summary>
        /// Check if the content contains a function call.
        /// </summary>
        /// <param name="content">A content</param>
        /// <returns>Returns true if the content contains a function call.</returns>
        public static bool ContainsFunctionCall(this Content content)
        {
            if (content.parts == null || content.parts.Count == 0)
            {
                return false;
            }
            foreach (var part in content.parts)
            {
                if (part.functionCall != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Build all FunctionDeclaration from [FunctionCall] in the script
        /// </summary>
        /// <param name="obj">Ayn type of object</param>
        /// <param name="flags">A binding flag.</param>
        /// <returns>Built FunctionDeclarations</returns>
        // TODO: Consider migrating to source generator
        public static Tool.FunctionDeclaration[] BuildFunctionsFromAttributes(
            this object obj,
            BindingFlags flags = DefaultBindings)
        {
            var methods = obj.GetType().GetMethods(flags)
                .Where(method => method.GetCustomAttribute<DescriptionAttribute>() != null);
            if (methods.Count() == 0)
            {
                throw new ArgumentException(NO_DESCRIPTION_ATTRIBUTE);
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
            string description = method.GetCustomAttribute<DescriptionAttribute>()?.Description
                ?? throw new ArgumentException(NO_DESCRIPTION_ATTRIBUTE);

            Type returnType = method.ReturnType;
            if (returnType.IsGenericTask())
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            // Debug.Log($"Method: {method.Name}, ReturnType: {returnType}");

            ParameterInfo[]? parameters = method.GetParameters()
                // Ignore CancellationToken
                .Where(p => p.ParameterType != typeof(CancellationToken))
                .ToArray();

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

        private static bool IsGenericTask(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        private static Tool.Schema ToSchema(this ParameterInfo parameter)
        {
            var schema = parameter.ParameterType.ToSchema(0);
            schema.description = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description;
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
                properties = toolType == Tool.Type.OBJECT ? type.GetFields(DefaultBindings).ToDictionary(
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
                Type when type == typeof(byte) => Tool.Type.INTEGER,
                Type when type == typeof(short) => Tool.Type.INTEGER,
                Type when type == typeof(sbyte) => Tool.Type.INTEGER,
                Type when type == typeof(uint) => Tool.Type.INTEGER,
                Type when type == typeof(ulong) => Tool.Type.INTEGER,
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

