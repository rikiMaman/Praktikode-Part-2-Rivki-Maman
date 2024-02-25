using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htmlSerializer
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public override string ToString()
        {
            string s = "";
            if (Name != null) s += "Name: " + Name;
            if (Id != null) s += " Id: " + Id;
            if (Classes.Count > 0)
            {
                s += " Classes: ";
                foreach (var clas in Classes)
                    s += clas + " ";
            }
            return s;
        }
        // פונקציה שמחזירה את כל האלמנטים התת-נכסים של אלמנט זה
        //פונקציה מסוג IEnumerable כי מחזירה  yield return
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement c = queue.Dequeue();
                if (this != c)
                {
                    yield return c;
                }
                //הכנסת כל הילדים של הילדים של האלמנט הנוכחי
                foreach (HtmlElement child in c.Children)
                {
                    queue.Enqueue(child);
                }


            }
           
        }
        // (לשורש יש אבא null) פונקציה שמחזירה את כל האבות של אלמנט זה עד לשורש
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
        // פונקציה שמוצאת אלמנטים מתאימים ל-selector בעץ התת-נכסים של אלמנט זה
        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            // מבנה מתאים כדי שלא  יהיה כפולים 
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            //מעבר על כל שבעץ
            foreach (var child in Descendants())
                child.FindElementsRecursively(selector, result);
            return result;
        }

        // פונקציה פרטית שבודקת רקורסיבית את התואמים ל-selector
        private void FindElementsRecursively(Selector selector, HashSet<HtmlElement> result)
        {
            if (!IsMatch(selector))
                return;

            //ירדנו הכי לעומק- כי אין לאלמנט הנוכחי בן
            if (selector.Child == null)
                result.Add(this);
            else
                foreach (var child in Descendants())
                    child.FindElementsRecursively(selector.Child, result);
        }
        // פונקציה פרטית שבודקת האם אלמנט זה תואם ל-selector
        private bool IsMatch(Selector selector)
        {
            return ((selector.TagName == null || Name.Equals(selector.TagName))
                && (selector.Id == null || selector.Id.Equals(Id))
                && (selector.Classes.Intersect(Classes).Count() == selector.Classes.Count));
        }


    }
}
