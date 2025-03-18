using System;
using GoogleApis.GenerativeLanguage;
using UnityEngine;
using Unity.Properties;

namespace GoogleApis.Example.UI.Components
{
    [Serializable]
    public partial class ContentItem
    {
        private Content content;
        public Content Content
        {
            get => content;
            set => content = value;
        }

        [field: SerializeField, DontCreateProperty]
        [CreateProperty]
        public string Text { get; set; }
    }
}
