using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ProjectSync
{
	public static class ProjectFileWriter
	{
        public static void UpdateProjectFileCompileList(string filePath, List<string> compileList)
        {
            // Read XML content
            string fileContents = File.ReadAllText(filePath);
            XDocument xdoc = XDocument.Parse(fileContents);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            XElement elProject = xdoc.Element(ns + "Project");
            List<XElement> elGlobalCompiles = xdoc.Descendants(ns + "Compile").ToList();
            if (elProject == null)
                throw new Exception("Could not find the Project xml node.");

            // Remove Compile nodes
            foreach (XElement elCompile in elGlobalCompiles)
                elCompile.Remove();

            // Add a ItemGroup with all compiles
            XElement elGlobalItemGroup = new XElement(ns + "ItemGroup");
            foreach (string compile in compileList)
            {
                XElement elCompile = new XElement(ns + "Compile");
                XAttribute attrInclude = new XAttribute("Include", compile);
                elCompile.Add(attrInclude);
                elGlobalItemGroup.Add(elCompile);
            }
            elProject.Add(elGlobalItemGroup);

            // Remove empty ItemGroup nodes
            List<XElement> elItemGroups = xdoc.Descendants(ns + "ItemGroup").ToList();
            foreach (XElement elItemGroup in elItemGroups)
            {
                if (!elItemGroup.HasElements)
                {
                    elItemGroup.Remove();
                }
            }

            // Save project file
            xdoc.Save(filePath);
        }

	}
}