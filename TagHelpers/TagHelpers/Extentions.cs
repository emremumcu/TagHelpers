namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extentions
    {
        public static void AddCssClass(this TagHelperOutput output, String cssClass)
        {
            AddCssClass(output, new List<string>() { cssClass });
        }

        public static void AddCssClass(this TagHelperOutput output, List<string> cssClasses)
        {
            Func<string, string, string> f_join 
                = new Func<string, string, string>((string s1, string s2) => string.Concat(s1, " ", s2));

            if (output.Attributes.ContainsName("class"))
            {
                string existingClassValues = output.Attributes["class"].Value?.ToString().Trim();
                if (!string.IsNullOrEmpty(existingClassValues)) cssClasses.Insert(0, existingClassValues);
            }

            // TagHelperAttribute classAttribute = new TagHelperAttribute("class", string.Join(" ", cssClasses));
            TagHelperAttribute classAttribute = new TagHelperAttribute("class", Enumerable.Aggregate<string>(cssClasses, f_join));

            output.Attributes.SetAttribute(classAttribute);
        }

        //public static void AddCssClass(this TagHelperOutput output, IEnumerable<string> cssClasses)
        //{
        //    if (output.Attributes.ContainsName("class") && output.Attributes["class"] != null)
        //    {
        //        List<string> classes = output.Attributes["class"].Value.ToString()!.Split(new char[1]
        //        {
        //            ' '
        //        }).ToList();
        //        foreach (string item in cssClasses.Where((string cssClass) => !classes.Contains(cssClass)))
        //        {
        //            classes.Add(item);
        //        }
        //        output.Attributes.SetAttribute("class", classes.Aggregate((string s, string s1) => s + " " + s1));
        //    }
        //    else if (output.Attributes.ContainsName("class"))
        //    {
        //        output.Attributes.SetAttribute("class", cssClasses.Aggregate((string s, string s1) => s + " " + s1));
        //    }
        //    else
        //    {
        //        output.Attributes.Add("class", cssClasses.Aggregate((string s, string s1) => s + " " + s1));
        //    }
        //}

        public static void RemoveCssClass(this TagHelperOutput output, string cssClass)
        {
            if (!output.Attributes.ContainsName("class"))
            {
                return;
            }
            List<string> list = output.Attributes["class"].Value.ToString()!.Split(new char[1]
            {
                ' '
            }).ToList();
            list.Remove(cssClass);
            if (list.Count == 0)
            {
                output.Attributes.RemoveAll("class");
                return;
            }
            output.Attributes.SetAttribute("class", list.Aggregate((string s, string s1) => s + " " + s1));
        }

        public static void AddCssStyle(this TagHelperOutput output, string name, string value)
        {
            if (output.Attributes.ContainsName("style"))
            {
                if (string.IsNullOrEmpty(output.Attributes["style"].Value.ToString()))
                {
                    output.Attributes.SetAttribute("style", name + ": " + value + ";");
                    return;
                }
                output.Attributes.SetAttribute("style", (output.Attributes["style"].Value.ToString()!.EndsWith(";") ? " " : "; ") + name + ": " + value + ";");
            }
            else
            {
                output.Attributes.Add("style", name + ": " + value + ";");
            }
        }

        public static void AddAriaAttribute(this TagHelperOutput output, string name, object value)
        {
            output.MergeAttribute("aria-" + name, value);
        }

        public static void AddDataAttribute(this TagHelperOutput output, string name, object value)
        {
            output.MergeAttribute("data-" + name, value);
        }

        public static void MergeAttribute(this TagHelperOutput output, string key, object value)
        {
            output.Attributes.SetAttribute(key, value);
        }
    }
}
