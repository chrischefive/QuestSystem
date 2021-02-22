using System;
using UnityEngine;


namespace Chrische.QuestSystem
{
    [Serializable]
    public class SubQuest : ScriptableObject
    {
        private string _name = String.Empty;
        private QuestStatus _status = QuestStatus.NOT_YET_GETTED;
        private string _description = String.Empty;
        private string _mission = String.Empty;
        private string _giver = String.Empty;
        private bool _isOptional = false;
        private int _exPoints = -1;

        public override string ToString()
        {
            return _name;
        }

        #region Properties

        public int ExPoints
        {
            get => _exPoints;
            set => _exPoints = value;
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

        public string Giver
        {
            get => _giver;
            set => _giver = value;
        }
        
        #endregion
    }
}

