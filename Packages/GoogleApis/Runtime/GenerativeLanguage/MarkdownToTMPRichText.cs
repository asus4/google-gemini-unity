using System;
using System.Text;

namespace GoogleApis.GenerativeLanguage
{
    /// <summary>
    /// Convert Markdown to TextMesh Pro Style Rich Text
    /// 
    /// https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html
    /// </summary>
    public static class MarkdownToTMPRichText
    {
        private const string H1_OPEN = "<size=2em><b>";
        private const string H1_CLOSE = "</b></size>";
        private const string H2_OPEN = "<size=1.5em><b>";
        private const string H2_CLOSE = "</b></size>";
        private const string H3_OPEN = "<size=1.17em><b>";
        private const string H3_CLOSE = "</b></size>";

        private static readonly StringBuilder sb = new();

        /// <summary>
        /// Convert Markdown to TextMesh Pro Style Rich Text
        /// </summary>
        /// <param name="text">Markdown Text</param>
        /// <returns>TextMesh Pro Rich Text</returns>
        public static string MarkdownToRichText(this string markdown)
        {
            sb.Clear();

            // TODO: consider using MemoryExtensions.Split
            var lines = markdown.Split("\n\n");
            foreach (var line in lines)
            {
                var lineSpan = line.AsSpan();

                // H1
                if (lineSpan.StartsWith("# "))
                {
                    sb.Append(H1_OPEN.AsSpan());
                    sb.Append(lineSpan[2..]);
                    sb.Append(H1_CLOSE.AsSpan());
                    sb.AppendLine();
                }
                // H2
                else if (lineSpan.StartsWith("## "))
                {
                    sb.Append(H2_OPEN.AsSpan());
                    sb.Append(lineSpan[3..]);
                    sb.Append(H2_CLOSE.AsSpan());
                    sb.AppendLine();
                }
                // H3
                else if (lineSpan.StartsWith("### "))
                {
                    sb.Append(H3_OPEN.AsSpan());
                    sb.Append(lineSpan[4..]);
                    sb.Append(H3_CLOSE.AsSpan());
                    sb.AppendLine();
                }
                // Unordered List
                else if (lineSpan.StartsWith("- "))
                {
                    ParseUnorderedList(sb, line, "- ");
                }
                else if (lineSpan.StartsWith("* "))
                {
                    ParseUnorderedList(sb, line, "* ");
                }
                // TODO: Implement: Ordered List
                // TODO: Implement: Code Block
                else
                {
                    ParseInline(sb, line);
                    // sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static void ParseUnorderedList(StringBuilder sb, in string markdown,
            ReadOnlySpan<char> markChar, char styleChar = '・')
        {
            var lines = markdown.Split('\n');
            foreach (var line in lines)
            {
                if (line.AsSpan().StartsWith(markChar))
                {
                    sb.Append(styleChar);
                    ParseInline(sb, line[2..]);
                    sb.AppendLine();
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(' ');
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
        }

        private static void ParseInline(StringBuilder sb, ReadOnlySpan<char> markdown)
        {
            bool isBold = false;
            bool isItalic = false;
            // bool isCode = false;

            for (int i = 0; i < markdown.Length; i++)
            {
                var current = markdown[i..];

                switch (markdown[i])
                {
                    case '*':
                        // Bold
                        if (current.StartsWith("**"))
                        {
                            if (isBold)
                            {
                                sb.Append("</b>".AsSpan());
                            }
                            else
                            {
                                sb.Append("<b>".AsSpan());
                            }
                            isBold = !isBold;
                            i++;
                            break;
                        }

                        // Italic
                        if (isItalic)
                        {
                            sb.Append("</i>".AsSpan());
                        }
                        else
                        {
                            sb.Append("<i>".AsSpan());
                        }
                        isItalic = !isItalic;
                        break;
                    // TODO: implement: Code 
                    // case '`':
                    //     if (current.StartsWith("```"))
                    //     {
                    //         // Excepts Code Block
                    //         i += 2;
                    //         break;
                    //     }
                    //     if (isCode)
                    //     {
                    //         sb.Append("</mspace>".AsSpan());
                    //     }
                    //     else
                    //     {
                    //         sb.Append("<mspace>".AsSpan());
                    //     }
                    //     isCode = !isCode;
                    //     break;
                    default:
                        sb.Append(markdown[i]);
                        break;
                }
            }
        }

        /// <summary>
        /// Append Content to StringBuilder as TextMesh Pro Rich Text Style
        /// </summary>
        /// <param name="sb">A StringBuilder</param>
        /// <param name="content">A Content</param>
        public static void AppendTMPRichText(this StringBuilder sb, Content content)
        {
            if (content.role.HasValue)
            {
                sb.AppendLine($"<b>{content.role.Value}:</b>");
            }
            foreach (var part in content.parts)
            {
                sb.AppendTMPRichText(part);
            }
        }

        /// <summary>
        /// Append Part to StringBuilder as TextMesh Pro Rich Text Style
        /// </summary>
        /// <param name="sb">A StringBuilder</param>
        /// <param name="part">A Content.Part</param>
        public static void AppendTMPRichText(this StringBuilder sb, Content.Part part)
        {
            string text = part switch
            {
                _ when !string.IsNullOrEmpty(part.text) => part.text.MarkdownToRichText(),
                { inlineData: not null } => $"inlineData: {part.inlineData.mimeType}",
                { functionCall: not null } => $"functionCall: {part.functionCall.name}",
                { functionResponse: not null } => $"functionResponse: {part.functionResponse.name}",
                { fileData: not null } => $"fileData: mime={part.fileData.mimeType}, uri={part.fileData.fileUri}",
                _ => $"<u><color=red>Unsupported part</color></u>",
            };
            sb.AppendLine(text);
        }
    }
}
