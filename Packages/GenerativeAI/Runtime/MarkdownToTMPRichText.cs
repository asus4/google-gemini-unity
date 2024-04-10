using System;
using System.Text;

namespace GenerativeAI
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

            var lines = markdown.Split("\n\n");
            foreach (var line in lines)
            {
                // H1
                if (line.StartsWith("# "))
                {
                    sb.Append(H1_OPEN.AsSpan());
                    sb.Append(line[2..].AsSpan());
                    sb.Append(H1_CLOSE.AsSpan());
                    sb.AppendLine();
                    sb.AppendLine();
                }
                // H2
                else if (line.StartsWith("## "))
                {
                    sb.Append(H2_OPEN.AsSpan());
                    sb.Append(line[3..].AsSpan());
                    sb.Append(H2_CLOSE.AsSpan());
                    sb.AppendLine();
                    sb.AppendLine();
                }
                // H3
                else if (line.StartsWith("### "))
                {
                    sb.Append(H3_OPEN.AsSpan());
                    sb.Append(line[4..].AsSpan());
                    sb.Append(H3_CLOSE.AsSpan());
                    sb.AppendLine();
                    sb.AppendLine();
                }
                // Unordered List
                else if (line.StartsWith("- "))
                {
                    ParseUnorderedList(sb, line, "- ");
                }
                else if (line.StartsWith("* "))
                {
                    ParseUnorderedList(sb, line, "* ");
                }
                else
                {
                    sb.Append(line);
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static void ParseUnorderedList(StringBuilder sb, string markdown,
            string markChar = "- ", string styleChar = "・")
        {
            var lines = markdown.Split("\n");
            foreach (var line in lines)
            {
                if (line.StartsWith(markChar))
                {
                    sb.Append(styleChar);
                    sb.Append(line[2..]);
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine("  ");
                }
            }
            sb.AppendLine();
        }
    }
}
