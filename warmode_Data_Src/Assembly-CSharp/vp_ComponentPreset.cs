using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public sealed class vp_ComponentPreset
{
	private enum ReadMode
	{
		Normal,
		LineComment,
		BlockComment
	}

	private class Field
	{
		public RuntimeFieldHandle FieldHandle;

		public object Args;

		public Field(RuntimeFieldHandle fieldHandle, object args)
		{
			this.FieldHandle = fieldHandle;
			this.Args = args;
		}
	}

	private static string m_FullPath;

	private static int m_LineNumber;

	private static Type m_Type;

	public static bool LogErrors = true;

	private static vp_ComponentPreset.ReadMode m_ReadMode;

	private Type m_ComponentType;

	private List<vp_ComponentPreset.Field> m_Fields = new List<vp_ComponentPreset.Field>();

	public Type ComponentType
	{
		get
		{
			return this.m_ComponentType;
		}
	}

	public static string Save(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(component);
		return vp_ComponentPreset.Save(vp_ComponentPreset, fullPath, false);
	}

	public static string Save(vp_ComponentPreset savePreset, string fullPath, bool isDifference = false)
	{
		vp_ComponentPreset.m_FullPath = fullPath;
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(vp_ComponentPreset.m_FullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null)
		{
			if (vp_ComponentPreset.m_ComponentType != null)
			{
				if (vp_ComponentPreset.ComponentType != savePreset.ComponentType)
				{
					return string.Concat(new string[]
					{
						"'",
						vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath),
						"' has the WRONG component type: ",
						vp_ComponentPreset.ComponentType.ToString(),
						".\n\nDo you want to replace it with a ",
						savePreset.ComponentType.ToString(),
						"?"
					});
				}
				if (File.Exists(vp_ComponentPreset.m_FullPath))
				{
					if (isDifference)
					{
						return "This will update '" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' with only the values modified since pressing Play or setting a state.\n\nContinue?";
					}
					return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' already exists.\n\nDo you want to replace it?";
				}
			}
			if (File.Exists(vp_ComponentPreset.m_FullPath))
			{
				return "'" + vp_ComponentPreset.ExtractFilenameFromPath(vp_ComponentPreset.m_FullPath) + "' has an UNKNOWN component type.\n\nDo you want to replace it?";
			}
		}
		vp_ComponentPreset.ClearTextFile();
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////");
		vp_ComponentPreset.Append("// Component Preset Script");
		vp_ComponentPreset.Append("///////////////////////////////////////////////////////////\n");
		vp_ComponentPreset.Append("ComponentType " + savePreset.ComponentType.Name);
		foreach (vp_ComponentPreset.Field current in savePreset.m_Fields)
		{
			string str = string.Empty;
			string text = string.Empty;
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(current.FieldHandle);
			if (fieldFromHandle.FieldType == typeof(float))
			{
				text = string.Format("{0:0.#######}", (float)current.Args);
			}
			else if (fieldFromHandle.FieldType == typeof(Vector4))
			{
				Vector4 vector = (Vector4)current.Args;
				text = string.Concat(new string[]
				{
					string.Format("{0:0.#######}", vector.x),
					" ",
					string.Format("{0:0.#######}", vector.y),
					" ",
					string.Format("{0:0.#######}", vector.z),
					" ",
					string.Format("{0:0.#######}", vector.w)
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector3))
			{
				Vector3 vector2 = (Vector3)current.Args;
				text = string.Concat(new string[]
				{
					string.Format("{0:0.#######}", vector2.x),
					" ",
					string.Format("{0:0.#######}", vector2.y),
					" ",
					string.Format("{0:0.#######}", vector2.z)
				});
			}
			else if (fieldFromHandle.FieldType == typeof(Vector2))
			{
				Vector2 vector3 = (Vector2)current.Args;
				text = string.Format("{0:0.#######}", vector3.x) + " " + string.Format("{0:0.#######}", vector3.y);
			}
			else if (fieldFromHandle.FieldType == typeof(int))
			{
				text = ((int)current.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(bool))
			{
				text = ((bool)current.Args).ToString();
			}
			else if (fieldFromHandle.FieldType == typeof(string))
			{
				text = (string)current.Args;
			}
			else
			{
				str = "//";
				text = "<NOTE: Type '" + fieldFromHandle.FieldType.Name.ToString() + "' can't be saved to preset.>";
			}
			if (!string.IsNullOrEmpty(text) && fieldFromHandle.Name != "Persist")
			{
				vp_ComponentPreset.Append(str + fieldFromHandle.Name + " " + text);
			}
		}
		return null;
	}

	public static string SaveDifference(vp_ComponentPreset initialStatePreset, Component modifiedComponent, string fullPath, vp_ComponentPreset diskPreset)
	{
		if (initialStatePreset.ComponentType != modifiedComponent.GetType())
		{
			vp_ComponentPreset.Error("Tried to save difference between different type components in 'SaveDifference'");
			return null;
		}
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.InitFromComponent(modifiedComponent);
		vp_ComponentPreset vp_ComponentPreset2 = new vp_ComponentPreset();
		vp_ComponentPreset2.m_ComponentType = vp_ComponentPreset.ComponentType;
		for (int i = 0; i < vp_ComponentPreset.m_Fields.Count; i++)
		{
			if (!initialStatePreset.m_Fields[i].Args.Equals(vp_ComponentPreset.m_Fields[i].Args))
			{
				vp_ComponentPreset2.m_Fields.Add(vp_ComponentPreset.m_Fields[i]);
			}
		}
		foreach (vp_ComponentPreset.Field current in diskPreset.m_Fields)
		{
			bool flag = true;
			foreach (vp_ComponentPreset.Field current2 in vp_ComponentPreset2.m_Fields)
			{
				if (current.FieldHandle == current2.FieldHandle)
				{
					flag = false;
				}
			}
			bool flag2 = false;
			foreach (vp_ComponentPreset.Field current3 in vp_ComponentPreset.m_Fields)
			{
				if (current.FieldHandle == current3.FieldHandle)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				flag = false;
			}
			if (flag)
			{
				vp_ComponentPreset2.m_Fields.Add(current);
			}
		}
		return vp_ComponentPreset.Save(vp_ComponentPreset2, fullPath, true);
	}

	public void InitFromComponent(Component component)
	{
		this.m_ComponentType = component.GetType();
		this.m_Fields.Clear();
		FieldInfo[] fields = component.GetType().GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string)))
			{
				this.m_Fields.Add(new vp_ComponentPreset.Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
	}

	public static vp_ComponentPreset CreateFromComponent(Component component)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.m_ComponentType = component.GetType();
		FieldInfo[] fields = component.GetType().GetFields();
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			if (fieldInfo.IsPublic && (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(Vector4) || fieldInfo.FieldType == typeof(Vector3) || fieldInfo.FieldType == typeof(Vector2) || fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(string)))
			{
				vp_ComponentPreset.m_Fields.Add(new vp_ComponentPreset.Field(fieldInfo.FieldHandle, fieldInfo.GetValue(component)));
			}
		}
		return vp_ComponentPreset;
	}

	public bool LoadTextStream(string fullPath)
	{
		vp_ComponentPreset.m_FullPath = fullPath;
		FileInfo fileInfo = new FileInfo(vp_ComponentPreset.m_FullPath);
		if (fileInfo == null || !fileInfo.Exists)
		{
			vp_ComponentPreset.Error("Failed to read file. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		TextReader textReader = fileInfo.OpenText();
		List<string> list = new List<string>();
		string item;
		while ((item = textReader.ReadLine()) != null)
		{
			list.Add(item);
		}
		textReader.Close();
		if (list == null)
		{
			vp_ComponentPreset.Error("Preset is empty. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		this.ParseLines(list);
		return true;
	}

	public static bool Load(Component component, string fullPath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		return vp_ComponentPreset.Apply(component, vp_ComponentPreset);
	}

	public bool LoadFromResources(string resourcePath)
	{
		vp_ComponentPreset.m_FullPath = resourcePath;
		TextAsset textAsset = Resources.Load(vp_ComponentPreset.m_FullPath) as TextAsset;
		if (textAsset == null)
		{
			vp_ComponentPreset.Error("Failed to read file. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		return this.LoadFromTextAsset(textAsset);
	}

	public static vp_ComponentPreset LoadFromResources(Component component, string resourcePath)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromResources(resourcePath);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	public bool LoadFromTextAsset(TextAsset file)
	{
		vp_ComponentPreset.m_FullPath = file.name;
		List<string> list = new List<string>();
		string[] array = file.text.Split(new char[]
		{
			'\n'
		});
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string item = array2[i];
			list.Add(item);
		}
		if (list == null)
		{
			vp_ComponentPreset.Error("Preset is empty. '" + vp_ComponentPreset.m_FullPath + "'");
			return false;
		}
		this.ParseLines(list);
		return true;
	}

	public static vp_ComponentPreset LoadFromTextAsset(Component component, TextAsset file)
	{
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(file);
		vp_ComponentPreset.Apply(component, vp_ComponentPreset);
		return vp_ComponentPreset;
	}

	private static void Append(string str)
	{
		str = str.Replace("\n", Environment.NewLine);
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(vp_ComponentPreset.m_FullPath, true);
			streamWriter.WriteLine(str);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			vp_ComponentPreset.Error("Failed to write to file: '" + vp_ComponentPreset.m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	private static void ClearTextFile()
	{
		StreamWriter streamWriter = null;
		try
		{
			streamWriter = new StreamWriter(vp_ComponentPreset.m_FullPath, false);
			if (streamWriter != null)
			{
				streamWriter.Close();
			}
		}
		catch
		{
			vp_ComponentPreset.Error("Failed to clear file: '" + vp_ComponentPreset.m_FullPath + "'");
		}
		if (streamWriter != null)
		{
			streamWriter.Close();
		}
	}

	private void ParseLines(List<string> lines)
	{
		vp_ComponentPreset.m_LineNumber = 0;
		foreach (string current in lines)
		{
			vp_ComponentPreset.m_LineNumber++;
			string text = vp_ComponentPreset.RemoveComments(current);
			if (!string.IsNullOrEmpty(text))
			{
				if (!this.Parse(text))
				{
					return;
				}
			}
		}
		vp_ComponentPreset.m_LineNumber = 0;
	}

	private bool Parse(string line)
	{
		line = line.Trim();
		if (string.IsNullOrEmpty(line))
		{
			return true;
		}
		string[] array = line.Split(null, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		if (this.m_ComponentType == null)
		{
			if (!(array[0] == "ComponentType") || array.Length != 2)
			{
				vp_ComponentPreset.PresetError("Unknown ComponentType.");
				return false;
			}
			vp_ComponentPreset.m_Type = Type.GetType(array[1]);
			if (vp_ComponentPreset.m_Type == null)
			{
				vp_ComponentPreset.PresetError("No such ComponentType: '" + array[1] + "'");
				return false;
			}
			this.m_ComponentType = vp_ComponentPreset.m_Type;
			return true;
		}
		else
		{
			FieldInfo fieldInfo = null;
			FieldInfo[] fields = vp_ComponentPreset.m_Type.GetFields();
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo fieldInfo2 = fields[j];
				if (fieldInfo2.Name == array[0])
				{
					fieldInfo = fieldInfo2;
				}
			}
			if (fieldInfo == null)
			{
				if (array[0] != "ComponentType")
				{
					vp_ComponentPreset.PresetError(string.Concat(new string[]
					{
						"'",
						vp_ComponentPreset.m_Type.Name,
						"' has no such field: '",
						array[0],
						"'"
					}));
				}
				return true;
			}
			vp_ComponentPreset.Field item = new vp_ComponentPreset.Field(fieldInfo.FieldHandle, vp_ComponentPreset.TokensToObject(fieldInfo, array));
			this.m_Fields.Add(item);
			return true;
		}
	}

	public static bool Apply(Component component, vp_ComponentPreset preset)
	{
		if (preset == null)
		{
			vp_ComponentPreset.Error("Tried to apply a preset that was null in '" + vp_Utility.GetErrorLocation(1) + "'");
			return false;
		}
		if (preset.m_ComponentType == null)
		{
			vp_ComponentPreset.Error("Preset ComponentType was null in '" + vp_Utility.GetErrorLocation(1) + "'");
			return false;
		}
		if (component == null)
		{
			vp_ComponentPreset.Error("Component was null when attempting to apply preset in '" + vp_Utility.GetErrorLocation(1) + "'");
			return false;
		}
		if (component.GetType() != preset.m_ComponentType)
		{
			string text = "a '" + preset.m_ComponentType + "' preset";
			if (preset.m_ComponentType == null)
			{
				text = "an unknown preset type";
			}
			vp_ComponentPreset.Error(string.Concat(new string[]
			{
				"Tried to apply ",
				text,
				" to a '",
				component.GetType().ToString(),
				"' component in '",
				vp_Utility.GetErrorLocation(1),
				"'"
			}));
			return false;
		}
		foreach (vp_ComponentPreset.Field current in preset.m_Fields)
		{
			FieldInfo[] fields = component.GetType().GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.FieldHandle == current.FieldHandle)
				{
					fieldInfo.SetValue(component, current.Args);
				}
			}
		}
		return true;
	}

	public static Type GetFileType(string fullPath)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadTextStream(fullPath);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	public static Type GetFileTypeFromAsset(TextAsset asset)
	{
		bool logErrors = vp_ComponentPreset.LogErrors;
		vp_ComponentPreset.LogErrors = false;
		vp_ComponentPreset vp_ComponentPreset = new vp_ComponentPreset();
		vp_ComponentPreset.LoadFromTextAsset(asset);
		vp_ComponentPreset.LogErrors = logErrors;
		if (vp_ComponentPreset != null && vp_ComponentPreset.m_ComponentType != null)
		{
			return vp_ComponentPreset.m_ComponentType;
		}
		return null;
	}

	private static object TokensToObject(FieldInfo field, string[] tokens)
	{
		if (field.FieldType == typeof(float))
		{
			return vp_ComponentPreset.ArgsToFloat(tokens);
		}
		if (field.FieldType == typeof(Vector4))
		{
			return vp_ComponentPreset.ArgsToVector4(tokens);
		}
		if (field.FieldType == typeof(Vector3))
		{
			return vp_ComponentPreset.ArgsToVector3(tokens);
		}
		if (field.FieldType == typeof(Vector2))
		{
			return vp_ComponentPreset.ArgsToVector2(tokens);
		}
		if (field.FieldType == typeof(int))
		{
			return vp_ComponentPreset.ArgsToInt(tokens);
		}
		if (field.FieldType == typeof(bool))
		{
			return vp_ComponentPreset.ArgsToBool(tokens);
		}
		if (field.FieldType == typeof(string))
		{
			return vp_ComponentPreset.ArgsToString(tokens);
		}
		return null;
	}

	private static string RemoveComments(string str)
	{
		string text = string.Empty;
		for (int i = 0; i < str.Length; i++)
		{
			switch (vp_ComponentPreset.m_ReadMode)
			{
			case vp_ComponentPreset.ReadMode.Normal:
				if (str[i] == '/' && str[i + 1] == '*')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.BlockComment;
					i++;
				}
				else if (str[i] == '/' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.LineComment;
					i++;
				}
				else
				{
					text += str[i];
				}
				break;
			case vp_ComponentPreset.ReadMode.LineComment:
				if (i == str.Length - 1)
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
				}
				break;
			case vp_ComponentPreset.ReadMode.BlockComment:
				if (str[i] == '*' && str[i + 1] == '/')
				{
					vp_ComponentPreset.m_ReadMode = vp_ComponentPreset.ReadMode.Normal;
					i++;
				}
				break;
			}
		}
		return text;
	}

	private static Vector4 ArgsToVector4(string[] args)
	{
		if (args.Length - 1 != 4)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector4.zero;
		}
		Vector4 result;
		try
		{
			result = new Vector4(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]), Convert.ToSingle(args[4]));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				", ",
				args[4],
				"'"
			}));
			return Vector4.zero;
		}
		return result;
	}

	private static Vector3 ArgsToVector3(string[] args)
	{
		if (args.Length - 1 != 3)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector3.zero;
		}
		Vector3 result;
		try
		{
			result = new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				", ",
				args[3],
				"'"
			}));
			return Vector3.zero;
		}
		return result;
	}

	private static Vector2 ArgsToVector2(string[] args)
	{
		if (args.Length - 1 != 2)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return Vector2.zero;
		}
		Vector2 result;
		try
		{
			result = new Vector2(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]));
		}
		catch
		{
			vp_ComponentPreset.PresetError(string.Concat(new string[]
			{
				"Illegal value: '",
				args[1],
				", ",
				args[2],
				"'"
			}));
			return Vector2.zero;
		}
		return result;
	}

	private static float ArgsToFloat(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0f;
		}
		float result;
		try
		{
			result = Convert.ToSingle(args[1]);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0f;
		}
		return result;
	}

	private static int ArgsToInt(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return 0;
		}
		int result;
		try
		{
			result = Convert.ToInt32(args[1]);
		}
		catch
		{
			vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
			return 0;
		}
		return result;
	}

	private static bool ArgsToBool(string[] args)
	{
		if (args.Length - 1 != 1)
		{
			vp_ComponentPreset.PresetError("Wrong number of fields for '" + args[0] + "'");
			return false;
		}
		if (args[1].ToLower() == "true")
		{
			return true;
		}
		if (args[1].ToLower() == "false")
		{
			return false;
		}
		vp_ComponentPreset.PresetError("Illegal value: '" + args[1] + "'");
		return false;
	}

	private static string ArgsToString(string[] args)
	{
		string text = string.Empty;
		for (int i = 1; i < args.Length; i++)
		{
			text += args[i];
			if (i < args.Length - 1)
			{
				text += " ";
			}
		}
		return text;
	}

	public Type GetFieldType(string fieldName)
	{
		Type result = null;
		foreach (vp_ComponentPreset.Field current in this.m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(current.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = fieldFromHandle.FieldType;
			}
		}
		return result;
	}

	public object GetFieldValue(string fieldName)
	{
		object result = null;
		foreach (vp_ComponentPreset.Field current in this.m_Fields)
		{
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(current.FieldHandle);
			if (fieldFromHandle.Name == fieldName)
			{
				result = current.Args;
			}
		}
		return result;
	}

	public static string ExtractFilenameFromPath(string path)
	{
		int num = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if (num == -1)
		{
			return path;
		}
		if (num == path.Length - 1)
		{
			return string.Empty;
		}
		return path.Substring(num + 1, path.Length - num - 1);
	}

	private static void PresetError(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError(string.Concat(new object[]
		{
			"Preset Error: ",
			vp_ComponentPreset.m_FullPath,
			" (at ",
			vp_ComponentPreset.m_LineNumber,
			") ",
			message
		}));
	}

	private static void Error(string message)
	{
		if (!vp_ComponentPreset.LogErrors)
		{
			return;
		}
		Debug.LogError("Error: " + message);
	}
}
