using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANSIApp : Program
{
    private float lineCount;
    private float charCount;
    private Computer host;

    public virtual void Start() {
        this.fileName = "ansi_app";
    }

    public override bool Execute(Computer host, out string output) {
        this.host = host;
        host.TerminalSession.ansiMode = true;
        return ANSIExecute(out output);
        // StartCoroutine(ANSIExecute(out output));
    }

    public virtual bool ANSIExecute(out string output) {
    // public virtual IEnumerable ANSIExecute(out string output) {
        output = "";
        lineCount = host.TerminalSession.LineCount;
        charCount = host.TerminalSession.CharCount;
        
        for (int y=0; y<=lineCount; y++) {
            for (int x=0; x<charCount; x++) {
                if (x == charCount-1 && y == lineCount) { output += "$"; continue; }
                output += "#";
            }
            output += "\n";
        }

        // host.TerminalSession.ansiMode = false;
        return true;
    }
}
