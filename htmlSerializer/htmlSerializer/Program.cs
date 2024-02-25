// See https://aka.ms/new-console-template for more information
using htmlSerializer;
using System.Text.RegularExpressions;
using System.Xml.Linq;
var html = await Load("https://chat.openai.com/c/56cd050d-711f-4446-b341-b1f370be600e");
html = new Regex("[\\r\\n\\t]").Replace(new Regex("\\s{2,}").Replace(html, ""), "");
var htmlLines = new Regex("<(.*?)>").Split(html).Where(x => x.Length > 0).ToArray();

HtmlElement root = CreateChild(htmlLines[1].Split(' ')[0], null, htmlLines[1]);
ParseHtml(root, htmlLines.Skip(2).ToList());
Console.WriteLine("HTML Tree:");
PrintHtmlTree(root, "");
//חיפוש אחר class שערכו הוא: "h2" ויש בתוכו id שערכו: challenge-error-text
//var list = root.FindElements(Selector.ParseSelectorString(".h2 #challenge-error-text"));
//foreach (var item in list)
//{
//    Console.WriteLine(item);
//}
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response= await client.GetAsync(url);
    var html=await response.Content.ReadAsStringAsync();
    return html;
}
static HtmlElement ParseHtml(HtmlElement rootElement, List<string> htmlLines)
{
    HtmlElement currentParent = rootElement;

    foreach (var line in htmlLines)
    {
        if (line.StartsWith("/html"))
            break;
        //נסגרה התגית - נחזור לאבא של הנוכחי
        if (line.StartsWith("/"))
        {
            currentParent = currentParent.Parent;
            continue;
        }
        string tagName = line.Split(' ')[0];
        if (!HtmlHelper.Instance.AllTags.Contains(tagName))
        {
            currentParent.InnerHtml += line;
            continue;
        }
        HtmlElement child = CreateChild(tagName, currentParent, line);
        currentParent.Children.Add(child);
        if (!HtmlHelper.Instance.SelfClosingTags.Contains(tagName) && !line.EndsWith("/"))
            currentParent = child;
    }
    return rootElement;
}
static HtmlElement CreateChild(string tagName, HtmlElement currentParent, string line)
{
    HtmlElement child = new HtmlElement { Name = tagName, Parent = currentParent };

    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
    foreach (var attr in attributes)
    {
        string attributeName = attr.ToString().Split('=')[0];
        string attributeValue = attr.ToString().Split('=')[1].Replace("\"", "");

        if (attributeName.ToLower() == "class")
            child.Classes.AddRange(attributeValue.Split(' '));
        else if (attributeName.ToLower() == "id")
            child.Id = attributeValue;
        else child.Attributes.Add(attributeName, attributeValue);
    }
    return child;
}
static void PrintHtmlTree(HtmlElement element, string indentation)
{
    Console.WriteLine($"{indentation}{element}");
    foreach (var child in element.Children)
        PrintHtmlTree(child, indentation + "  ");
}

