
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chrische.QuestSystem
{
    public class QuestSystem : EditorWindow
    {
        private string _name = String.Empty;
        private int _statusIndex = 0;
        private string _description = String.Empty;
        private string _mission = String.Empty;
        private GameObject _giver = default;
        private bool _isOptional = false;
        private List<Quest> _subQuests = new List<Quest>();
        private int _selectedSubQuestIndex = 0;
        private static int _minLevel = -10;
        private static int _maxLevel = -1;
        private static int _exPoints = -20;
        private List<GameObject> _rewards = new List<GameObject>();

        private string _subQuestName = String.Empty;
        private string _subQuestMission = String.Empty;
        private string _subQuestDescription = String.Empty;
        private GameObject _subQuestGiver = default;
        private List<GameObject> _subQuestRewards = new List<GameObject>();

        private bool _isAddingSubQuest = false;
        private bool _isEditingSubQuest = false;
        private Quest _tempQuest = null;

        private bool _isContentShown = false;
        
        [MenuItem("Window/QuestSystem")]
        public static void ShowWindow()
        {
            GetWindow<QuestSystem>("QuestSystem");
        }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Quest-Data");
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Quest"))
            {
                _isContentShown = true;
            }
            EditorGUILayout.LabelField("or");
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Edit Quest:");
                _tempQuest = EditorGUILayout.ObjectField(_tempQuest, typeof(Quest), false) as Quest;
            }
            EditorGUILayout.EndHorizontal();
            if (_tempQuest)
            {
                _name = _tempQuest.Name;
                _statusIndex = (int)_tempQuest.Status;
                _description = _tempQuest.Description;
                _mission = _tempQuest.Mission;
                _giver = _tempQuest.Giver;
                _isOptional = _tempQuest.IsOptional;
                _minLevel = _tempQuest.MinLevel;
                _maxLevel = _tempQuest.MaxLevel;
                _exPoints = _tempQuest.ExPoints;
                _rewards = _tempQuest.Rewards;
                _subQuests = _tempQuest.SubQuests;
                _isContentShown = true;
            }

            EditorGUILayout.Space();
            DrawUiLine(Color.gray);
            EditorGUILayout.Space();
            if (_isContentShown)
            {
                DrawContent();
            }

            if (GUILayout.Button("Clear all"))
            {
                _isContentShown = false;
                _isAddingSubQuest = false;
                _isEditingSubQuest = false;
                _tempQuest = null;
                _description = String.Empty;
                _name = String.Empty;
                _subQuests.Clear();
            }
        }

        private void DrawContent()
        {
            DrawParentQuest();
        }

        private void DrawParentQuest()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Name:");
                _name = EditorGUILayout.TextField(_name);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                var allStatus = new List<GUIContent>();
                foreach (QuestStatus status in Enum.GetValues(typeof(QuestStatus)))
                {
                    var content = new GUIContent(status.ToString());
                    allStatus.Add(content);
                }
                _statusIndex = EditorGUILayout.Popup(new GUIContent("Status:"), _statusIndex, allStatus.ToArray());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Description:");
                _description = EditorGUILayout.TextArea(_description);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Mission:");
                _mission = EditorGUILayout.TextField(_mission);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Giver:");
                _giver = EditorGUILayout.ObjectField(_tempQuest, typeof(GameObject), true) as GameObject;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Min Level");
                _minLevel = EditorGUILayout.IntField(_minLevel);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Max Level");
                _maxLevel = EditorGUILayout.IntField(_maxLevel);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Experience Points");
                _exPoints = EditorGUILayout.IntField(_exPoints);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Is optional:");
                _isOptional = EditorGUILayout.Toggle(_isOptional);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            DrawUiLine(Color.grey, 1);
            EditorGUILayout.LabelField("Subquests");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                var allSubQuest = new List<string>(); 
                foreach (var subQuest in _subQuests)
                {
                    allSubQuest.Add(subQuest.ToString());
                }
                _selectedSubQuestIndex = EditorGUILayout.Popup("SubQuest to edit", _selectedSubQuestIndex, allSubQuest.ToArray());
                if (GUILayout.Button("Edit"))
                {
                    _isEditingSubQuest = true;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add Subquest"))
            {
                _isAddingSubQuest = true;
                _subQuestName = String.Empty;
                _subQuestMission = String.Empty;
                _subQuestDescription = String.Empty;
                _subQuestGiver = default;
                _subQuestRewards.Clear();
                Repaint();
            }
            

            if (_isAddingSubQuest)
            {
                DrawSubQuest(null);
            }
            else if(_isEditingSubQuest)
            {
                DrawSubQuest(_subQuests[_selectedSubQuestIndex]);
            }
            
            EditorGUILayout.Space();
            DrawUiLine(Color.grey, 1);
            EditorGUILayout.Space();
            if (GUILayout.Button("Save Quest"))
            {
                
                var questToSave = GetQuestToSave();
                var path = "Assets/" + questToSave.Name + "-quest.asset";
                AssetDatabase.CreateAsset(questToSave, path);
                AssetDatabase.SaveAssets();
            }
        }

        private Quest GetQuestToSave()
        {
            var questToSave = CreateInstance<Quest>();
            questToSave.Name = _name;
            questToSave.Description = _description;
            questToSave.Giver = _giver;
            questToSave.Rewards = _rewards;
            questToSave.Status = (QuestStatus) _statusIndex;
            questToSave.ExPoints = _exPoints;
            questToSave.IsOptional = _isOptional;
            questToSave.MaxLevel = _maxLevel;
            questToSave.MinLevel = _minLevel;
            questToSave.SubQuests = _subQuests;
            questToSave.Mission = _mission;
            return questToSave;
        }
        
        private void DrawSubQuest(Quest quest)
        {
            if (quest != null)
            {
                FillSubQuest(quest);
            }
            DrawUiLine(Color.grey, 1);
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Name:");
                        _subQuestName = EditorGUILayout.TextField(_subQuestName);
                        
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Description:");
                        _subQuestDescription = EditorGUILayout.TextField(_subQuestDescription);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Mission:");
                        _subQuestMission = EditorGUILayout.TextField(_subQuestMission);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Giver:");
                        _subQuestGiver = EditorGUILayout.ObjectField(_subQuestGiver, typeof(GameObject), true) as GameObject;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Save SubQuest"))
            {
                var subQuest = CreateInstance<Quest>();
                subQuest.Name = _subQuestName;
                subQuest.Mission = _subQuestMission;
                subQuest.Description = _subQuestDescription;
                subQuest.Giver = _subQuestGiver;
                
                _subQuests.Add(subQuest);
                ClearSubQuest();
                _selectedSubQuestIndex = _subQuests.Count - 1;
                _isAddingSubQuest = false;
                _isEditingSubQuest = false;
                Repaint();
            }
            EditorGUILayout.Space();
            DrawUiLine(Color.grey, 1);
            EditorGUILayout.Space();
        }

        private void ClearSubQuest()
        {
            _subQuestName = String.Empty;
            _subQuestDescription = String.Empty;
            _subQuestMission = String.Empty;
            _subQuestGiver = default;
            _subQuestRewards = new List<GameObject>();
        }

        private void FillSubQuest(Quest quest)
        {
            _subQuestName = quest.Name;
            _subQuestDescription = quest.Description;
            _subQuestMission = quest.Mission;
            _subQuestGiver = quest.Giver;
            _subQuestRewards = quest.Rewards;
        }
        
        private static void DrawUiLine(Color color, int thickness = 2, int padding = 10)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += (int)(padding / 2f);
            rect.x -= 2;
            EditorGUI.DrawRect(rect, color);
        }
    }
}

