#if UNITY_EDITOR
using System;
using System.Reflection;
using Object = UnityEngine.Object;

#if CSHARP_7_3_OR_NEWER
using Sisus.Vexe.FastReflection;
#endif

namespace Sisus
{
	/// <summary> Can be used for drawing the asset bundle GUI for target objects in the editor. </summary>
	public class AssetBundleGUIDrawer
	{
		private object[] onAssetBundleNameGUIParams = new object[1];
		private object assetBundleNameGUI;
		#if CSHARP_7_3_OR_NEWER
		private MethodCaller<object, object> onAssetBundleNameGUI;
		#else
		private MethodInfo onAssetBundleNameGUI;
		#endif
		
		public bool HasTarget
		{
			get
			{
				return onAssetBundleNameGUIParams[0] != null;
			}
		}

		public AssetBundleGUIDrawer()
		{
			#if UNITY_EDITOR
			var type = Types.GetInternalEditorType("UnityEditor.AssetBundleNameGUI");
			assetBundleNameGUI = Activator.CreateInstance(type);
			#if CSHARP_7_3_OR_NEWER
			onAssetBundleNameGUI = type.GetMethod("OnAssetBundleNameGUI", BindingFlags.Public | BindingFlags.Instance).DelegateForCall();
			#else
			onAssetBundleNameGUI = type.GetMethod("OnAssetBundleNameGUI", BindingFlags.Public | BindingFlags.Instance);
			#endif
			#endif
		}
		
		public void ResetState()
		{
			onAssetBundleNameGUIParams[0] = null;
		}

		public void SetAssets(Object[] targets)
		{
			onAssetBundleNameGUIParams[0] = targets;
		}

		public void Draw()
		{
			#if CSHARP_7_3_OR_NEWER
			onAssetBundleNameGUI(assetBundleNameGUI, onAssetBundleNameGUIParams);
			#else
			onAssetBundleNameGUI.Invoke(assetBundleNameGUI, onAssetBundleNameGUIParams);
			#endif
		}
	}
}
#endif