#define SUPPORT_EDITORS_FOR_INTERFACES // the default inspector doesn't support this but we can

//#define DEBUG_CUSTOM_EDITORS
//#define DEBUG_PROPERTY_DRAWERS
//#define DEBUG_SET_EDITING_TEXT_FIELD

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using Sisus.Compatibility;

#if DEV_MODE && DEBUG_CUSTOM_EDITORS
using System.Linq;
#endif

namespace Sisus
{
	public static class CustomEditorUtility
	{
		private static Dictionary<Type, Type> customEditorsByType;
		private static Dictionary<Type, Type> propertyDrawersByType;
		private static Dictionary<Type, Type> decoratorDrawersByType;

		public static Dictionary<Type, Type> CustomEditorsByType
		{
			get
			{
				if(customEditorsByType == null)
				{
					customEditorsByType = new Dictionary<Type, Type>();

					var inspectedTypeField = typeof(CustomEditor).GetField("m_InspectedType", BindingFlags.NonPublic | BindingFlags.Instance);
					var useForChildrenField = typeof(CustomEditor).GetField("m_EditorForChildClasses", BindingFlags.NonPublic | BindingFlags.Instance);

					IEnumerable<Type> editorTypes;
					var ignored = PluginAttributeConverterProvider.ignoredEditors;

					//NOTE: important to also get invisible types, so that internal Editors such as RectTransformEditor are also returned
					if(ignored == null || ignored.Count == 0)
					{
						editorTypes = TypeExtensions.GetAllTypesThreadSafe(false, true, true).Where((t) => t.IsSubclassOf(Types.Editor));
					}
					else
					{
						editorTypes = TypeExtensions.GetAllTypesThreadSafe(false, true, true).Where((t) => t.IsSubclassOf(Types.Editor) && !ignored.Contains(t));
					}

					#if DEV_MODE
					if(!editorTypes.Contains(Types.GetInternalEditorType("UnityEditor.RectTransformEditor"))) { Debug.LogError("RectTransformEditor was not among "+ editorTypes.Count()+" Editors!"); };
					#endif

					try
					{
						GetDrawersByInspectedTypeFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, ref customEditorsByType, false);

						// Second pass: also apply for inheriting types if they don't already have more specific overrides.
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, useForChildrenField, ref customEditorsByType, true, false);
					}
					#if DEV_MODE
					catch(Exception e)
					{
						Debug.LogWarning(e);
					#else
					catch(Exception)
					{
					#endif
						// Do slow but safe loading with try-catch used for every type separately to figure out which type caused the exception.
						// This should also inform the user about the Type causing issues as well as instructions on how to deal with it.
						if(ignored == null || ignored.Count == 0)
						{
							editorTypes = TypeExtensions.GetExtendingTypesThreadSafeExceptionSafeSlow(Types.Editor, false, true, true);
						}
						else
						{
							editorTypes = TypeExtensions.GetExtendingTypesThreadSafeExceptionSafeSlow(Types.Editor, false, true, true).Where((t)=>!ignored.Contains(t));
						}

						customEditorsByType.Clear();
						GetDrawersByInspectedTypeFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, ref customEditorsByType, false);
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomEditor>(editorTypes, inspectedTypeField, useForChildrenField, ref customEditorsByType, true, false);
					}

					#if DEV_MODE // TEMP
					if(TryGetCustomEditorType(typeof(UnityEngine.Object), out var logType)) { Debug.Log("Object default Editor: "+logType.FullName); }
					if(TryGetCustomEditorType(typeof(Transform), out logType)) { Debug.Log("Transform Editor: "+logType.FullName); }
					if(TryGetCustomEditorType(typeof(MonoBehaviour), out logType)) { Debug.Log("MonoBehaviour Editor: " + logType.FullName); }
					#endif

					#if DEV_MODE && DEBUG_CUSTOM_EDITORS
					var log = customEditorsByType.Where(pair => !Types.Component.IsAssignableFrom(pair.Key));
					Debug.Log("Non-Components with custom editors:\n"+StringUtils.ToString(log, "\n"));
					#endif
				}

				return customEditorsByType;
			}
		}

		public static Dictionary<Type, Type> PropertyDrawersByType
		{
			get
			{
				if(propertyDrawersByType == null)
				{
					propertyDrawersByType = new Dictionary<Type, Type>();

					var typeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
					var useForChildrenField = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);
					var propertyDrawerTypes = TypeExtensions.GetAllTypesThreadSafe(false, true, true).Where((type) => type.IsSubclassOf(typeof(UnityEditor.PropertyDrawer)));

					#if DEV_MODE && PI_ASSERTATIONS
					if(!propertyDrawerTypes.Contains(typeof(UnityEditorInternal.UnityEventDrawer))) { Debug.LogError("UnityEventDrawer not found among "+ propertyDrawerTypes.Count()+" PropertyDrawer types:\n"+StringUtils.ToString(propertyDrawerTypes, "\n")); };
					#endif

					try
					{
						GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, ref propertyDrawersByType, true);
						//second pass: also apply for inheriting types if they don't already have more specific overrides
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, useForChildrenField, ref propertyDrawersByType, false, true);
					}
					#if DEV_MODE
					catch(Exception e)
					{
						Debug.LogWarning(e);
					#else
					catch(Exception)
					{
					#endif
						// Do slow but safe loading with try-catch used for every type separately to figure out which type caused the exception.
						// This should also inform the user about the Type causing issues as well as instructions on how to deal with it.
						propertyDrawerTypes = TypeExtensions.GetExtendingTypesThreadSafeExceptionSafeSlow(typeof(UnityEditor.PropertyDrawer), false, true, true);

						propertyDrawersByType.Clear();
						GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, ref propertyDrawersByType, true);
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(propertyDrawerTypes, typeField, useForChildrenField, ref propertyDrawersByType, false, true);
					}

					#if DEV_MODE && PI_ASSERTATIONS
					if(!propertyDrawersByType.Values.Contains(typeof(UnityEditorInternal.UnityEventDrawer))) { Debug.LogError("UnityEventDrawer not found among "+ propertyDrawersByType.Count+" PropertyDrawers. "); };
					#endif

					#if DEV_MODE && DEBUG_PROPERTY_DRAWERS
					Debug.Log("propertyDrawersByType:\r\n"+StringUtils.ToString(propertyDrawersByType, "\r\n"));
					#endif
				}
				
				return propertyDrawersByType;
			}
		}

		public static Dictionary<Type, Type> DecoratorDrawersByType
		{
			get
			{
				if(decoratorDrawersByType == null)
				{
					decoratorDrawersByType = new Dictionary<Type, Type>();

					var typeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);

					// UPDATE: Apparently this field is not really respected for decorator drawers
					//var useForChildrenField = propertyDrawerType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

					var decoratorDrawerTypes = TypeExtensions.GetAllTypesThreadSafe(false, true, true).Where((type) => type.IsSubclassOf(typeof(DecoratorDrawer)));

					try
					{
						GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, ref decoratorDrawersByType, false);

						// Second pass: also apply for inheriting types if they don't already have more specific overrides.
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, null, ref decoratorDrawersByType, false, false);
					}
					#if DEV_MODE
					catch(Exception e)
					{
						Debug.LogWarning(e);
					#else
					catch(Exception)
					{
					#endif
						// Do slow but safe loading with try-catch used for every type separately to figure out which type caused the exception.
						// This should also inform the user about the Type causing issues as well as instructions on how to deal with it.
						decoratorDrawerTypes = TypeExtensions.GetExtendingTypesThreadSafeExceptionSafeSlow(typeof(DecoratorDrawer), false, true, true);

						decoratorDrawersByType.Clear();
						GetDrawersByInspectedTypeFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, ref decoratorDrawersByType, false);
						GetDrawersByInheritedInspectedTypesFromAttributes<CustomPropertyDrawer>(decoratorDrawerTypes, typeField, null, ref decoratorDrawersByType, false, false);
					}
				}
				
				return decoratorDrawersByType;
			}
		}
		
		/// <summary>
		/// Attempts to get PropertyDrawer Type for given class or from attributes on the field
		/// </summary>
		/// <param name="classMemberType"> Type of the class for which we are trying to find the PropertyDrawer. </param>
		/// <param name="memberInfo"> LinkedMemberInfo of the property for which we are trying to find the PropertyDrawer. </param>
		/// <param name="propertyAttribute"> [out] PropertyAttribute found on the property. </param>
		/// <param name="drawerType"> [out] Type of the PropertyDrawer for the PropertyAttribute. </param>
		/// <returns>
		/// True if target has a PropertyDrawer, false if not.
		/// </returns>
		public static bool TryGetPropertyDrawerType([NotNull]Type classMemberType, [NotNull]LinkedMemberInfo memberInfo, out PropertyAttribute propertyAttribute, out Type drawerType)
		{
			var attributes = memberInfo.GetAttributes(Types.PropertyAttribute);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				if(TryGetPropertyDrawerType(attribute.GetType(), out drawerType))
				{
					propertyAttribute = attribute as PropertyAttribute;
					return true;
				}
			}
			propertyAttribute = null;
			return TryGetPropertyDrawerType(classMemberType, out drawerType);
		}

		/// <summary>
		/// Attempts to get PropertyDrawer Type for given class.
		/// </summary>
		/// <param name="classMemberOrAttributeType"> Type of the class for which we are trying to find the PropertyDrawer. </param>
		/// <param name="propertyDrawerType"> [out] Type of the PropertyDrawer. </param>
		/// <returns> True if target has a PropertyDrawer, false if not. </returns>
		public static bool TryGetPropertyDrawerType([NotNull]Type classMemberOrAttributeType, out Type propertyDrawerType)
		{
			if(PropertyDrawersByType.TryGetValue(classMemberOrAttributeType, out propertyDrawerType))
			{
				return true;
			}

			if(classMemberOrAttributeType.IsGenericType && !classMemberOrAttributeType.IsGenericTypeDefinition)
			{
				return propertyDrawersByType.TryGetValue(classMemberOrAttributeType.GetGenericTypeDefinition(), out propertyDrawerType);
			}

			return false;
		}

		/// <summary>
		/// Does class member attribute of given type have a PropertyDrawer?
		/// </summary>
		/// <param name="classMemberOrAttributeType"> Type of class member or attribute. </param>
		/// <returns> True if class has a PropertyDrawer, false if not. </returns>
		public static bool HasPropertyDrawer([NotNull]Type classMemberOrAttributeType)
		{
			if(PropertyDrawersByType.ContainsKey(classMemberOrAttributeType))
			{
				return true;
			}

			if(classMemberOrAttributeType.IsGenericType && !classMemberOrAttributeType.IsGenericTypeDefinition)
			{
				return propertyDrawersByType.ContainsKey(classMemberOrAttributeType.GetGenericTypeDefinition());
			}

			return false;
		}

		public static bool TryGetDecoratorDrawerTypes([NotNull]LinkedMemberInfo memberInfo, out object[] decoratorAttributes, out Type[] drawerTypes)
		{
			//TO DO: Add support for PropertyAttribute.order
			
			drawerTypes = null;
			decoratorAttributes = null;
			
			var attributes = memberInfo.GetAttributes(Types.PropertyAttribute);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				Type drawerType;
				if(TryGetDecoratorDrawerType(attribute.GetType(), out drawerType))
				{
					if(drawerTypes == null)
					{
						decoratorAttributes = new[] { attribute };
						drawerTypes = new[]{drawerType};
					}
					else
					{
						decoratorAttributes = decoratorAttributes.Add(attribute);
						drawerTypes = drawerTypes.Add(drawerType);
					}
				}
			}
			
			return drawerTypes != null;
		}

		public static bool TryGetDecoratorDrawerTypes([NotNull]MemberInfo memberInfo, out object[] decoratorAttributes, out Type[] drawerTypes)
		{
			//TO DO: Add support for PropertyAttribute.order
			// 
			drawerTypes = null;
			decoratorAttributes = null;
			
			var attributes = memberInfo.GetCustomAttributes(Types.PropertyAttribute, true);
			for(int n = attributes.Length - 1; n >= 0; n--)
			{
				var attribute = attributes[n];
				Type drawerType;
				if(TryGetDecoratorDrawerType(attribute.GetType(), out drawerType))
				{
					if(drawerTypes == null)
					{
						decoratorAttributes = new[] { attribute };
						drawerTypes = new[]{drawerType};
					}
					else
					{
						decoratorAttributes = decoratorAttributes.Add(attribute);
						drawerTypes = drawerTypes.Add(drawerType);
					}
				}
			}
			
			return drawerTypes != null;
		}
		
		public static bool AttributeHasDecoratorDrawer(Type propertyAttributeType)
		{
			if(DecoratorDrawersByType.ContainsKey(propertyAttributeType))
			{
				return true;
			}

			if(propertyAttributeType.IsGenericType && !propertyAttributeType.IsGenericTypeDefinition)
			{
				return decoratorDrawersByType.ContainsKey(propertyAttributeType.GetGenericTypeDefinition());
			}

			return false;
		}

		public static bool TryGetDecoratorDrawerType(Type propertyAttributeType, out Type decoratorDrawerType)
		{
			if(DecoratorDrawersByType.TryGetValue(propertyAttributeType, out decoratorDrawerType))
			{
				return true;
			}

			if(propertyAttributeType.IsGenericType && !propertyAttributeType.IsGenericTypeDefinition)
			{
				return decoratorDrawersByType.TryGetValue(propertyAttributeType.GetGenericTypeDefinition(), out decoratorDrawerType);
			}

			return false;
		}
		
		public static bool TryGetCustomEditorType([NotNull]Type targetType, out Type editorType)
		{
			if(CustomEditorsByType.TryGetValue(targetType, out editorType))
			{
				return true;
			}

			if(targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
			{
				return customEditorsByType.TryGetValue(targetType.GetGenericTypeDefinition(), out editorType);
			}

			return false;
		}

		/// <summary>
		/// Given an array of PropertyDrawer, DecoratorDrawers or Editors, gets their inspected types and adds them to drawersByInspectedType.
		/// </summary>
		/// <typeparam name="TAttribute"> Type of the attribute. </typeparam>
		/// <param name="drawerOrEditorTypes"> List of PropertyDrawer, DecoratorDrawer or Editor types. </param>
		/// <param name="targetTypeField"> FieldInfo for getting the inspected type. </param>
		/// <param name="drawersByInspectedType">
		/// [in,out] dictionary where drawer types will be added with their inspected types as the keys. </param>
		private static void GetDrawersByInspectedTypeFromAttributes<TAttribute>([NotNull]IEnumerable<Type> drawerOrEditorTypes, [NotNull]FieldInfo targetTypeField, [NotNull]ref Dictionary<Type,Type> drawersByInspectedType, bool canBeAbstract) where TAttribute : Attribute
		{
			var attType = typeof(TAttribute);
			
			foreach(var drawerOrEditorType in drawerOrEditorTypes)
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(!drawerOrEditorType.IsAbstract);
				#endif

				var attributes = drawerOrEditorType.GetCustomAttributes(attType, true);
				for(int a = attributes.Length - 1; a >= 0; a--)
				{
					var attribute = attributes[a];
					var inspectedType = targetTypeField.GetValue(attribute) as Type;
					if(!inspectedType.IsAbstract || canBeAbstract)
					{
						if(!drawersByInspectedType.ContainsKey(inspectedType))
						{
							#if DEV_MODE // TEMP
							if(inspectedType == typeof(Transform)) { Debug.Log($"Transform Editor: "+ drawerOrEditorType.FullName); }
							else if(inspectedType == typeof(MonoBehaviour)) { Debug.Log($"MonoBehaviour Editor: " + drawerOrEditorType.FullName); }
							#endif

							drawersByInspectedType.Add(inspectedType, drawerOrEditorType);
						}
						else if(IsHigherPriority(drawerOrEditorType, drawersByInspectedType[inspectedType]))
						{
							#if DEV_MODE
							Debug.LogWarning($"Replacing {inspectedType.Name} old Editor {drawersByInspectedType[inspectedType].FullName} with new editor {drawerOrEditorType.FullName}.");
							#endif

							drawersByInspectedType[inspectedType] = drawerOrEditorType;
						}
						#if DEV_MODE
						else if(drawerOrEditorType != drawersByInspectedType[inspectedType]) { Debug.LogWarning($"Won't use Editor {drawerOrEditorType.FullName} for {inspectedType.Name} because already using {drawersByInspectedType[inspectedType].FullName}."); }
						#endif
					}
				}
			}
		}

		private static bool IsHigherPriority(Type compare, Type compareTo)
		{
			return GetPriority(compare) < GetPriority(compareTo);
		}

		/// <summary>
		/// Gets priority order for custom editor, property drawer or decorator drawer.
		/// Lower is better.
		/// </summary>
		/// <param name="type"> Custom editor, property drawer or decorator drawer type. </param>
		/// <returns> Priority order; lower is better. </returns>
		private static int GetPriority(Type type)
		{
			if(TypeExtensions.IsUnityAssemblyThreadSafe(type.Assembly))
			{
				return 3;
			}
			// Sisus assembly has medium priority
			if(type.Assembly == typeof(CustomEditorUtility).Assembly)
			{
				return 2;
			}
			// Other assemblies have highest priority
			return 1;
		}

		private static void GetDrawersByInheritedInspectedTypesFromAttributes<TAttribute>([NotNull]IEnumerable<Type> drawerOrEditorTypes, [NotNull]FieldInfo targetTypeField, [CanBeNull]FieldInfo useForChildrenField, [NotNull]ref Dictionary<Type,Type> drawersByInspectedType, bool targetMustBeUnityObject, bool canBeAbstract) where TAttribute : Attribute
		{
			var attType = typeof(TAttribute);
			foreach(var drawerType in drawerOrEditorTypes)
			{
				#if DEV_MODE && PI_ASSERTATIONS
				Debug.Assert(!drawerType.IsAbstract);
				#endif

				var attributes = drawerType.GetCustomAttributes(attType, true);
				for(int a = attributes.Length - 1; a >= 0; a--)
				{
					var attribute = attributes[a];

					bool useForChildren = useForChildrenField == null ? true : (bool)useForChildrenField.GetValue(attribute);
					if(!useForChildren)
					{
						#if DEV_MODE
						if(typeof(DecoratorDrawer).IsAssignableFrom(drawerType)) { Debug.LogWarning(drawerType.Name+ ".useForChildren was "+StringUtils.False); }
						#endif
						continue;
					}

					var targetType = targetTypeField.GetValue(attribute) as Type;

					if(targetType.IsClass)
					{
						var allTypes = TypeExtensions.GetAllTypesThreadSafe(canBeAbstract, true, true);
						Func<Type, bool> isSubclassOfTarget = (t) => t.IsSubclassOfUndeclaredGeneric(targetType);
						if(targetType.IsGenericTypeDefinition)
						{
							isSubclassOfTarget = (t) => t.IsSubclassOfUndeclaredGeneric(targetType);
						}
						else
						{
							isSubclassOfTarget = (t) => t.IsSubclassOf(targetType);
						}

						try
						{
							foreach(var extendingType in allTypes.Where(isSubclassOfTarget))
							{
								if(!drawersByInspectedType.ContainsKey(extendingType))
								{
									drawersByInspectedType.Add(extendingType, drawerType);
								}
							}
						}
						#if DEV_MODE
						catch(Exception e)
						{
							Debug.LogWarning(e);
						#else
						catch(Exception)
						{
						#endif
							// Do slow but safe loading with try-catch used for every type separately to figure out which type caused the exception.
							// This should also inform the user about the Type causing issues as well as instructions on how to deal with it.
							foreach(var extendingType in TypeExtensions.GetExtendingTypesThreadSafeExceptionSafeSlow(targetType, canBeAbstract, true, true))
							{
								if(!drawersByInspectedType.ContainsKey(extendingType))
								{
									drawersByInspectedType.Add(extendingType, drawerType);
								}
							}
						}
						continue;
					}

					// Value types don't support inheritance
					if(!targetType.IsInterface)
					{
						continue;
					}

					var implementingTypes = targetMustBeUnityObject ? targetType.GetImplementingUnityObjectTypes(true, canBeAbstract) : targetType.GetImplementingTypes(true, canBeAbstract);

					#if DEV_MODE && DEBUG_INTERFACE_SUPPORT
					Debug.Log("interface "+targetType.Name+" implementing types: "+StringUtils.ToString(implementingTypes));
					#endif

					for(int t = implementingTypes.Length - 1; t >= 0; t--)
					{
						var implementingType = implementingTypes[t];

						#if DEV_MODE && PI_ASSERTATIONS
						Debug.Assert(!implementingType.IsAbstract || canBeAbstract);
						#endif

						if(!drawersByInspectedType.ContainsKey(implementingType))
						{
							#if DEV_MODE && DEBUG_INTERFACE_SUPPORT
							Debug.Log("Adding interface "+targetType.Name+" implementing type "+StringUtils.ToString(implementingType) +"...");
							#endif

							drawersByInspectedType.Add(implementingType, drawerType);
						}
					}

					#if SUPPORT_EDITORS_FOR_INTERFACES
					if(targetMustBeUnityObject)
					{
						drawersByInspectedType.Add(targetType, drawerType);
					}
					#endif
				}
			}
		}

		public static void BeginEditor(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			BeginEditorOrPropertyDrawer(out editingTextFieldWas, out eventType, out keyCode);
		}

		public static void EndEditor(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			EndEditorOrPropertyDrawer(editingTextFieldWas, eventType, keyCode);
		}

		public static void BeginPropertyDrawer(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			BeginEditorOrPropertyDrawer(out editingTextFieldWas, out eventType, out keyCode);
		}
		

		public static void EndPropertyDrawer(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			EndEditorOrPropertyDrawer(editingTextFieldWas, eventType, keyCode);
		}

		private static void BeginEditorOrPropertyDrawer(out bool editingTextFieldWas, out EventType eventType, out KeyCode keyCode)
		{
			editingTextFieldWas = EditorGUIUtility.editingTextField;
			eventType = DrawGUI.LastInputEventType;
			var lastInputEvent = DrawGUI.LastInputEvent();
			keyCode = lastInputEvent == null ? KeyCode.None : lastInputEvent.keyCode;
		}

		private static void EndEditorOrPropertyDrawer(bool editingTextFieldWas, EventType eventType, KeyCode keyCode)
		{
			if(EditorGUIUtility.editingTextField != editingTextFieldWas)
			{
				if(eventType != EventType.KeyDown && eventType != EventType.KeyUp)
				{
					#if DEV_MODE && DEBUG_SET_EDITING_TEXT_FIELD
					Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(EditorGUIUtility.editingTextField)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
					#endif
					DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
				}
				else
				{
					switch(keyCode)
					{
						case KeyCode.UpArrow:
						case KeyCode.DownArrow:
						case KeyCode.LeftArrow:
						case KeyCode.RightArrow:
							if(!EditorGUIUtility.editingTextField)
							{
								#if DEV_MODE
								Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
								#endif
								DrawGUI.EditingTextField = false;
							}
							else // prevent Unity automatically starting field editing when field focus is changed to a text field, as that is not how Power Inspector functions
							{
								#if DEV_MODE
								Debug.LogWarning("EditorGUIUtility.editingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
								#endif
								EditorGUIUtility.editingTextField = false;
							}
							return;
						default:
							#if DEV_MODE
							Debug.Log("DrawGUI.EditingTextField = "+StringUtils.ToColorizedString(false)+" with eventType="+StringUtils.ToString(eventType)+", keyCode="+keyCode+", lastInputEvent="+StringUtils.ToString(DrawGUI.LastInputEvent()));
							#endif
							DrawGUI.EditingTextField = EditorGUIUtility.editingTextField;
							return;
					}
				}
			}
		}
	}
}
#endif