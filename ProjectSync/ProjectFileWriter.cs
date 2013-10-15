// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

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
