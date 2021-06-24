using System.Collections.Generic;
using System;
namespace Celeste.Mod.CherryHelper
{
    public class SavedFlagsData
    {
        public string campaignId;
        public Dictionary<string, bool> savedFlags;
        public SavedFlagsData(string campaignId, Dictionary<string, bool> savedFlags)
        {
            this.campaignId = campaignId;
            this.savedFlags = savedFlags;
        }
        public SavedFlagsData()
        {
            
        }

        public void handleSavedFlags(Session session)
        {
            foreach (string key in savedFlags.Keys)
            {
                savedFlags.TryGetValue(key, out bool value);
                session.SetFlag(key, value);
                Console.WriteLine("Flag " + key + "set to state " + value);
            }
        }
    }

    public class CherryHelperSaveData : EverestModuleSaveData
    {
        public List<SavedFlagsData> allSavedFlagsData { get; internal set; } = new List<SavedFlagsData>();
    }
}
