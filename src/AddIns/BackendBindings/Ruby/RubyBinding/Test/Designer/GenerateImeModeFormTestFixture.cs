﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class GenerateImeModeFormTestFixture
	{
		string generatedRubyCode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor doubleBufferedPropertyDescriptor = descriptors.Find("ImeMode", false);
				doubleBufferedPropertyDescriptor.SetValue(form, ImeMode.Alpha);

				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {					
					RubyCodeDomSerializer serializer = new RubyCodeDomSerializer("    ");
					generatedRubyCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, String.Empty, 1);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "    self.SuspendLayout()\r\n" +
								"    # \r\n" +
								"    # MainForm\r\n" +
								"    # \r\n" +
								"    self.ClientSize = System::Drawing::Size.new(284, 264)\r\n" +
								"    self.ImeMode = System::Windows::Forms::ImeMode.Alpha\r\n" +
								"    self.Name = \"MainForm\"\r\n" +
								"    self.ResumeLayout(false)\r\n";
			
			Assert.AreEqual(expectedCode, generatedRubyCode);
		}
	}
}
