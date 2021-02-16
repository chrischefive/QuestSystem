//#define DEBUG_ENABLED

using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace Sisus
{
	public enum FontSize
	{
		Normal = 0,
		Small = 1,
		Big = 2,
		Bold = 3
	}

	[InitializeOnLoad]
	public static class Fonts
	{
		public static Font Normal
		{
			get;
			private set;
		}

		public static Font Small
		{
			get;
			private set;
		}

		public static Font Big
		{
			get;
			private set;
		}

		public static Font Bold
		{
			get;
			private set;
		}
		
		public static FontCharSizes NormalSizes
		{
			get;
			private set;
		}

		public static FontCharSizes SmallSizes
		{
			get;
			private set;
		}

		public static bool SetupDone
		{
			get
			{
				return Normal != null;
			}
		}

		/// <summary>
		/// This is called on editor load because of usage of the InitializeOnLoad attribute.
		/// </summary>
		[UsedImplicitly]
		static Fonts()
		{
			EditorApplication.delayCall += DelayedSetup;
		}
		
		private static void DelayedSetup()
		{
			if(!ApplicationUtility.IsReady())
			{
				EditorApplication.delayCall += DelayedSetup;
				return;
			}

			DrawGUI.OnNextBeginOnGUI(SetupIfNotAlreadyDone, false);
		}

		/// <summary>
		/// This is called when entering play mode or when the game is loaded.
		/// </summary>
		[RuntimeInitializeOnLoadMethod, UsedImplicitly]
		private static void RuntimeInitializeOnLoad()
		{
			DrawGUI.OnNextBeginOnGUI(SetupIfNotAlreadyDone, false);
		}

		public static void SetupIfNotAlreadyDone()
		{
			if(!SetupDone)
			{
				Setup();
			}
		}

		/// <summary>
		/// Setups this class. Called during OnGUI.
		/// </summary>
		internal static void Setup()
		{
			Normal = EditorStyles.standardFont;
			Bold = EditorStyles.boldFont;
			Small = EditorStyles.miniFont;
			Big = (Font)EditorGUIUtility.LoadRequired("Fonts/Lucida Grande Big.ttf");

			if(Big == null)
			{
				Big = Normal;
			}
			
			#if DEV_MODE && PI_ASSERTATIONS
			Debug.Assert(Normal != null);
			Debug.Assert(Small != null);
			Debug.Assert(Big != null);
			Debug.Assert(Bold != null);
			#endif

			NormalSizes = new FontCharSizes(Normal, EditorStyles.label.fontSize);
			SmallSizes = new FontCharSizes(Small, EditorStyles.miniButton.fontSize);
			
			#if DEV_MODE && DEBUG_ENABLED
			Debug.Log("Normal="+Normal.name+", Small="+Small.name+", Big="+Big.name+", Bold="+Bold.name);
			#endif
		}

		public static Font GetFont(FontSize fontStyle)
		{
			switch(fontStyle)
			{
				case FontSize.Normal:
					return Normal;
				case FontSize.Small:
					return Small;
				case FontSize.Big:
					return Big;
				case FontSize.Bold:
					return Bold;
				default:
					throw new System.IndexOutOfRangeException(fontStyle.ToString());
			}
		}
	}
}