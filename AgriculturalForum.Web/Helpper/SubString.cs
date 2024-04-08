using System.Text;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using AgriculturalForum.Web.Models;

namespace AgriculturalForum.Web
{
    public class SubString
    {
        public static string ExtractTextFromHtml(string htmlContent, int maxLength)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var sb = new StringBuilder();
            int currentLength = 0;

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                string nodeText = node.InnerText.Trim() + " ";
                if (!string.IsNullOrEmpty(nodeText))
                {
                    if (currentLength + nodeText.Length > maxLength)
                    {
                        int remainingLength = maxLength - currentLength;
                        sb.Append(nodeText.Substring(0, remainingLength));
                        break;
                    }
                    else
                    {
                        sb.Append(nodeText);
                        currentLength += nodeText.Length;
                    }
                }
            }

            return sb.ToString();
        }

    }
}
