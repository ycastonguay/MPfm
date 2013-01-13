using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ProjectSync
{
	public static class ProjectFileReader
	{
	    public static List<string> ExtractProjectCompileFilePaths(string projectFilePath)
	    {
            // Read XML content
	        List<string> filePaths = new List<string>();
            string fileContents = File.ReadAllText(projectFilePath);
            XDocument xdoc = XDocument.Parse(fileContents);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            XElement elProject = xdoc.Element(ns + "Project");
            List<XElement> elItemGroups = xdoc.Descendants(ns + "ItemGroup").ToList();
            List<XElement> elGlobalCompiles = xdoc.Descendants(ns + "Compile").ToList();
            if(elProject == null)
                throw new Exception("Could not find the Project xml node.");

            foreach (XElement elItemGroup in elItemGroups)
            {
                List<XElement> elCompiles = elItemGroup.Descendants(ns + "Compile").ToList();
                foreach (XElement elCompile in elCompiles)
                {
                    XAttribute include = elCompile.Attribute("Include");
                    if (include != null)
                    {
                        filePaths.Add(include.Value);
                    }
                }
            }

	        return filePaths;
	    }
	}
}