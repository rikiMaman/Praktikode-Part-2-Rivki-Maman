using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace htmlSerializer
{
    public class HtmlHelper
    {
        //implement singleton:
        private static readonly HtmlHelper instance = new HtmlHelper();
        //return the singleton outside the class if needed:
        public static HtmlHelper Instance
        {
            get { return instance; }
        }

        //שמות כל התגיות
        public List<string> AllTags { get; private set; }
        //שמות כל התגיות ללא סגירה
        public List <string> SelfClosingTags { get; private set; }

        private HtmlHelper()
        {
            //טעינת כל התגיות ל 2 הרשימות בהתאמה
            string allTagsJson = File.ReadAllText("seed/HtmlTags.json");
            string selfClosingTagsJson = File.ReadAllText("seed/HtmlVoidTags.json");

            AllTags = JsonSerializer.Deserialize<List<string>>(allTagsJson);
            SelfClosingTags = JsonSerializer.Deserialize<List<string>>(selfClosingTagsJson);
        }
    }


}




