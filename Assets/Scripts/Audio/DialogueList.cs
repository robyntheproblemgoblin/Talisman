using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Talisman/DialogueList", fileName = "New Dialogue List")]
public class DialogueList : ScriptableObject
{
    public string m_section;
    public List<AudioSubtitle> m_audioList;
}

