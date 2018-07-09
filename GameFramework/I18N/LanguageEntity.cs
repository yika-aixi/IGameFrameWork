using System;
using System.Collections.Generic;

namespace Icarus.GameFramework.I18N
{
    public class LanguageEntity
    {
        public Dictionary<string, string> LanguageTable { get; private set; }

        public LanguageEntity(Dictionary<string, string> languageTable)
        {
            LanguageTable = languageTable;
        }

        public string GetValue(string key)
        {
            if (!LanguageTable.ContainsKey(key))
            {
                return string.Empty;
            }

            return LanguageTable[key];
        }

        public void AddAndUpdateLanguageTable(Dictionary<string, string> languageTable)
        {
            foreach (var pair in languageTable)
            {
                if (LanguageTable.ContainsKey(pair.Key))
                {
                    LanguageTable[pair.Key] = pair.Value;
                }
                else
                {
                    LanguageTable.Add(pair.Key,pair.Value);
                }
            }
        }

        public void Dispose()
        {
            LanguageTable.Clear();
        }
    }
}