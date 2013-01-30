using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace xpathr
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: xpathr.exe xmlFilePath xpath [namespacePrefix,url[;namespacePrefix,url]]");
                return;
            }

            string xmlfile = args[0];
            string xpath = args[1];
            //string ns = String.IsNullOrEmpty(args[2].Trim()) ? null : args[2].Trim();
            string[] namespaces = null;
            if (args.Length == 3)
            {
                namespaces = args[2].Trim().Split(';');
            }

            XPathDocument doc = null;
            try
            {
                doc = new XPathDocument(xmlfile);
            }
            catch (XmlException e)
            {
                Console.WriteLine(Path.GetFullPath(xmlfile));
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            XPathNavigator navigator = doc.CreateNavigator();
            XPathExpression expression = null;
            try
            {
                expression = navigator.Compile(xpath);
            }
            catch (XPathException exception)
            {
                Console.WriteLine(String.Format("There's a problem with the Xpath '{0}' \n{1}", xpath, exception.Message));
                Environment.Exit(1);
            }

            if(namespaces != null)
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
                foreach (string ns in namespaces)
                {
                    var nsInfo = ns.Split(',');
                    manager.AddNamespace(nsInfo[0], nsInfo[1]);
                }
                expression.SetContext(manager);
            }

            switch (expression.ReturnType)
            {
                case XPathResultType.Error:
                    Console.WriteLine(String.Format("There's a problem with the Xpath '{0}'", xpath));
                    break;
                case XPathResultType.NodeSet:
                    XPathNodeIterator nodes = navigator.Select(expression);
                    while (nodes.MoveNext())
                    {
                        if (nodes.Current != null)
                        {
                            Console.WriteLine(nodes.Current.OuterXml);
                        }
                    }

                    break;
                default:
                    Console.WriteLine(navigator.Evaluate(expression));
                    break;
            }
        }
    }
}
