using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Sisus.CreateScriptWizard
{
	[Serializable]
	internal class ScriptPrescription
	{
		public string nameSpace = "";
		public string className = "";
		public string template = "";
		public string baseClass = "";

		[NotNull]
		public string[] usingNamespaces = new string[0];
		
		[CanBeNull]
		public FunctionData[] functions;

		[NotNull]
		public readonly Dictionary<string, string> stringReplacements = new Dictionary<string, string> ();

		public ScriptPrescription() {}

		public void SetTemplate(string template)
		{
			this.template = template;
			ScriptTemplateUtility.TryGetBaseClass(template, out baseClass);
		}
	}
	
	internal struct FunctionData
	{
		public string attribute;
		public string prefix;
		public string name;
		public string returnType;
		public string returnDefault;
		[CanBeNull]
		public ParameterData[] parameters;
		public string comment;
		public bool isMethod;
		public bool include;

		public bool IsHeader
		{
			get
			{
				return name == null;
			}
		}

		public string HeaderName
		{
			get
			{
				return comment;
			}
		}

		public FunctionData(string headerName)
		{
			attribute = "";
			prefix = "";
			comment = headerName;
			name = null;
			returnType = null;
			returnDefault = null;
			parameters = null;
			include = false;
			isMethod = true;
		}
	}
	
	internal struct ParameterData
	{
		public string name;
		public string type;
		
		public ParameterData(string name, string type)
		{
			this.name = name;
			this.type = type;
		}
	}
}