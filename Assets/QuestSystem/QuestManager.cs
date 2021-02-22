using System.Collections.Generic;
using UnityEngine;

namespace Chrische.QuestSystem
{
    public class QuestManager
    {
        private List<Quest> _allQuest = new List<Quest>();

        public void AddQuest(Quest newQuest)
        {
            if (!_allQuest.Contains(newQuest))
            {
                _allQuest.Add(newQuest);
            }
            else
            {
                Debug.Log("#QuestManager#: try to add a quest, which is already in list");
            }
        }

        public List<Quest> GetQuests(params QuestStatus[] status)
        {
            var questsToReturn = new List<Quest>();
            foreach (var quest in _allQuest)
            {
                foreach (var s in status)
                {
                    if (quest.Status == s)
                    {
                        if (!questsToReturn.Contains(quest))
                        {
                            questsToReturn.Add(quest);
                        }
                    }
                }
            }

            return questsToReturn;
        }
        
    }
}

