using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TerminalManager : MonoBehaviour
{
    public TMP_Text terminalText;
    public void ResetTerminal() {
        Terminal currentTerminal = GetComponent<Terminal>();
        Terminal newTerminal = gameObject.AddComponent<Terminal>();
        newTerminal.baseHost = GameObject.Find("my_pc").GetComponent<Computer>();
        newTerminal.terminalText = terminalText;
        Destroy(currentTerminal);
    }    
}
