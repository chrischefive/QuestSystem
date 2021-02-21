using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chrische.QuestSystem
{
    public class Quest : ScriptableObject
    {
        private string _name = String.Empty;
        private QuestStatus _status = QuestStatus.NOT_YET_GETTED;
        private string _description = String.Empty;
        private string _mission = String.Empty;
        private GameObject _giver = default;
        private bool _isOptional = false;
        private int _minLevel = -1;
        private int _maxLevel = -1;
        private int _exPoints = -1;
        private List<GameObject> _rewards = new List<GameObject>();
        private List<Quest> _subQuests = new List<Quest>();

        public override string ToString()
        {
            return _name;
        }

        #region Properties

        public int MinLevel
        {
            get => _minLevel;
            set => _minLevel = value;
        }

        public int MaxLevel
        {
            get => _maxLevel;
            set => _maxLevel = value;
        }

        public int ExPoints
        {
            get => _exPoints;
            set => _exPoints = value;
        }

        public List<GameObject> Rewards
        {
            get => _rewards;
            set => _rewards = value;
        }

        public List<Quest> SubQuests
        {
            get => _subQuests;
            set => _subQuests = value;
        }

        public bool IsOptional
        {
            get => _isOptional;
            set => _isOptional = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }
    
        public QuestStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public string Mission
        {
            get => _mission;
            set => _mission = value;
        }

        public GameObject Giver
        {
            get => _giver;
            set => _giver = value;
        }

        #endregion
    }
}

