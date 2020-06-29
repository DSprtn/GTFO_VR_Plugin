using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace GTFO_VR
{
	/// <summary>
	/// Just a bit of logging tomfoolery. It would probably be better to implement some real-time console of sorts to display GameObject data in a better way.
	/// </summary>
    public static class DebugHelper
    {
		
		public static void LogScene()
		{
			foreach (Transform transform in UnityEngine.Object.FindObjectsOfType<Transform>())
			{
				if (transform.parent == null)
				{
					Debug.Log("RootObject ---\n");
					LogTransformHierarchy(transform);
				}
			}
		}

		public static void LogPosRotData(Transform t)
		{
			Debug.Log(t.name + ": " + "---" + GetTransformPositionAndRotationString(t));
		}
		public static string GetTransformPositionAndRotationString(Transform t)
		{
			return "Pos: " + t.position + "Rot: " + t.rotation.eulerAngles + "\n LocalRot: " + t.localRotation.eulerAngles + "LocalPos: " + t.localPosition + " LocalScale " + t.localScale + "Lossy scale " + t.lossyScale;
		}

		public static void LogTransformHierarchy(Transform t)
		{
			Debug.Log(GetTransformData(t, 0));
		}

		static string GetCurrentTransformInfo(Transform t, int depth)
		{
			string text = GetTabs(depth) + t.name;
			foreach (Component c in t.GetComponents<Component>())
			{
				text = string.Concat(new string[]
				{
				text,
				"\n",
				GetTabs(depth),
				"|_| ",
				GetTypeStrIfExists(c)
				});
			}
			return text + GetTransformPositionAndRotationString(t);
		}

		static string GetTransformData(Transform t, int depth)
		{
			string text = "";
			text += GetCurrentTransformInfo(t, depth);
			foreach (object obj in t)
			{
				Transform t2 = (Transform)obj;
				text = text + "\n" + GetTransformData(t2, depth + 1);
			}
			return text;
		}

		static string GetTypeStrIfExists(Component c)
		{
			if (c != null && c.GetType() != null)
			{
				return "T:" + c.GetType().ToString();
			}
			return "";
		}

		static string GetTabs(int depth)
		{
			string text = "";
			for (int i = 0; i < depth; i++)
			{
				text += "___|";
			}
			return text;
		}
	}
}
