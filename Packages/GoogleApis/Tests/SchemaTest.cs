using System.ComponentModel;
using GoogleApis.GenerativeLanguage;
using NUnit.Framework;
using UnityEngine;

using Description = System.ComponentModel.DescriptionAttribute;

namespace GoogleApis.Tests
{
    public class SchemaTest : MonoBehaviour
    {
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
            Debug.Log(schema);

            Assert.AreEqual(schema.Properties.Count, 3);
            foreach (var prop in schema.Properties.Values)
            {
                Assert.IsNotEmpty(prop.Description);
            }

        }
    }
}
