#nullable enable

using System;
using System.Linq;
using System.Reflection;

namespace Gemini
{
    /// <summary>
    /// Extension methods for calling functions using reflection.
    /// </summary>
    public static class FunctionCallingExtensions
    {


        public static object? InvokeFunctionCall(this object obj, Content.FunctionCall functionCall)
        {
            MethodInfo method = obj.GetType().GetMethod(functionCall.name)
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

        public static object? InvokeFunctionCall(this object obj, Content content)
        {
            if (content.FindFunctionCall() is Content.FunctionCall functionCall)
            {
                return obj.InvokeFunctionCall(functionCall);
            }
            throw new ArgumentException("No function call found in content");
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
    }
}
