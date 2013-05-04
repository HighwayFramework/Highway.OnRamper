using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OnRamper
{
    public class ConfigExaminer
    {
        public XDocument Populate(string package, string sXml)
        {
            var doc = XDocument.Parse(sXml);
            var exactComment = String.Format("[[{0}]]", package);
            var comments = doc.DescendantNodes()
                .OfType<XComment>()
                .Where(e => e.Value.Trim().Equals(exactComment, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            var targetNodes = comments.Select(e => e.NodesAfterSelf().OfType<XElement>().First()).ToList();

            var requiredNodes = new List<XNode>(targetNodes.DescendantNodesAndSelf());
            foreach (var target in targetNodes)
            {
                var parent = target.Parent;
                while (parent != null)
                {
                    requiredNodes.Add(parent);
                    parent = parent.Parent;
                }
            }

            IEnumerable<XNode> descendants = doc.DescendantNodes().ToList();
            foreach (var descendant in descendants)
            {
                if (requiredNodes.Contains(descendant) == false)
                {
                    descendant.Remove();
                }
            }

            return doc;
        }
    }
}
