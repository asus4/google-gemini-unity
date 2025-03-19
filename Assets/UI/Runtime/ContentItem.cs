using System;
using GoogleApis.GenerativeLanguage;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace LlmUI
{
    [Serializable]
    public class ContentItem
    {
        [field: SerializeField, DontCreateProperty]
        public Role Role { get; set; } = Role.model;

        [field: SerializeField, DontCreateProperty]
        [CreateProperty]
        public string Text { get; set; }

        [CreateProperty]
        public FlexDirection ContainerFlexDirection
        {
            get
            {
                return Role switch
                {
                    Role.user => FlexDirection.RowReverse,
                    Role.model => FlexDirection.Row,
                    _ => FlexDirection.Column,
                };
            }
        }

        public ContentItem()
        {
        }

        public ContentItem(Content content)
        {
            Role = content.Role ?? Role.model;
            Text = content.Parts[0].Text;
        }    
    }
}
