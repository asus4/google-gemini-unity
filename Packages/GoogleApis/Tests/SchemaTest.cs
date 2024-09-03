using System.ComponentModel;
using GoogleApis.GenerativeLanguage;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using Description = System.ComponentModel.DescriptionAttribute;

namespace GoogleApis.Tests
{
    public class SchemaTest : MonoBehaviour
    {
        private static readonly JsonSerializerSettings jsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public class TestClass1
        {
            [Description("Description of the text field")]
            public string text;
            [Description("Description of the integer field")]
            public int integerNumber;
            [Description("Description of the floating point number field")]
            public float floatingNumber;
        }

        [Test]
        public void TestCase1()
        {
            var schema = typeof(TestClass1).ToSchema();
            // Debug.Log(ToJSON(schema));

            Assert.AreEqual(schema.properties.Count, 3);
            foreach (var prop in schema.properties.Values)
            {
                Assert.IsNotEmpty(prop.description);
            }

        }

        private static string ToJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSettings);
        }
    }
}
