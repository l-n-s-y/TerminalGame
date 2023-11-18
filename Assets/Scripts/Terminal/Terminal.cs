using static System.Exception;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Terminal : MonoBehaviour
{
    // TODO:
    // - Command history                    DONE
    // - Tab completion                     
    // - Left/Right char navigation
    //      This can be done by indexing through currentLine
    //      but the result of just doing this without regard
    //      for the offsets of other characters will look shit

    // Possible Rich Text color names:
    // black, blue, green, orange, purple, red, white, yellow

    private Computer currentHost; // Either local or remotely connected PC
    private List<Computer> previousHosts; // Previous PC in connection chain
    public Computer baseHost; // Local PC
    public TMP_Text terminalText;

    private string promptColor = "#F00";
    
    private int lineCount; // Max lines per screen
    public int LineCount {
        get { return lineCount; }
    }
    private int charCount; // Max chars per line
    public int CharCount {
        get { return charCount; }
    }

    // private string prompt = "<color={0}>$></color>";
    private string prompt = "$>";
    private string currentLine;
    private List<string> previousLines;

    private int currentCharOffset = 0;

    public bool ansiMode = false;
    public ANSIApp ansiApp = null;

    private List<string> commandCache; // For up-arrow/down-arrow navigation
    private int cacheOffset = 0; // commandCache[i-cacheOffset] - up = +1 / down = -1
    private bool cacheSet = false;

    private string bannerColor = "yellow";
    // private string welcomeBanner = "<color={0}>====[ TERMEX 3.7 ]====</color>\nWelcome.\n";
    private string welcomeBanner = "<color={0}>====[ TERMEX 3.7 ]{1}</color>\nWelcome.\n";

    private List<(string,string)> keycodes;

    // Start is called before the first frame update
    bool booted = false;
    bool booting = false;
    void Start()
    {
        // terminalText.text = "a\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na\na";

        // Janky vertical overflow detection to get max line count
        terminalText.text = "";
        int maxLineCount = 255;
        for (int i=0; i<maxLineCount; i++) {
            terminalText.text += "a\n";
            terminalText.ForceMeshUpdate();
            if (terminalText.isTextOverflowing) {
                lineCount = i;
                // terminalText.text = "";
                // Remove last line for horizontal checking
                terminalText.text = terminalText.text.Substring(0,terminalText.text.Length-4);
                terminalText.ForceMeshUpdate();
                break;
            }
        }


        // Janky horizontal overflow detection
        // terminalText.text = string.concat(Enumerable.Repeat("a\n",lineCount));
        int maxCharCount = 255;
        for (int i=0; i<maxCharCount; i++) {
            if (terminalText.isTextOverflowing) {
                // If text is overflowing, 1 char less will be the maximum line char width
                charCount = i-1;
                terminalText.text = "";
                break;
            }
            terminalText.text += "a";
            terminalText.ForceMeshUpdate();
        }

        // Prompt colouring
        // prompt = string.Format(prompt,promptColor);
        Debug.Log(charCount);
        string bannerPadding = new string('=',charCount-new string("</color>\nWelcome.\n").Length);
        welcomeBanner = string.Format(welcomeBanner,bannerColor,bannerPadding);

        // String.Concat(Enumerable.Repeat("Hello", 4))

        previousHosts = new List<Computer>();

        currentHost = baseHost;
        string a;
        currentHost.Connect(this,"password",out a);
        currentHost.SetHostName("MyPC");

        currentLine = prompt;
        previousLines = new List<string>();

        commandCache = new List<string>();

        string[] welcomeLines = welcomeBanner.Split("\n");
        foreach (string line in welcomeLines) {
            previousLines.Add(line);
        }

        keycodes = new List<(string,string)>();
        keycodes.Add(("a","A"));
        keycodes.Add(("b","B"));
        keycodes.Add(("c","C"));
        keycodes.Add(("d","D"));
        keycodes.Add(("e","E"));
        keycodes.Add(("f","F"));
        keycodes.Add(("g","G"));
        keycodes.Add(("h","H"));
        keycodes.Add(("i","I"));
        keycodes.Add(("j","J"));
        keycodes.Add(("k","K"));
        keycodes.Add(("l","L"));
        keycodes.Add(("m","M"));
        keycodes.Add(("n","N"));
        keycodes.Add(("o","O"));
        keycodes.Add(("p","P"));
        keycodes.Add(("q","Q"));
        keycodes.Add(("r","R"));
        keycodes.Add(("s","S"));
        keycodes.Add(("t","T"));
        keycodes.Add(("u","U"));
        keycodes.Add(("v","V"));
        keycodes.Add(("w","W"));
        keycodes.Add(("x","X"));
        keycodes.Add(("y","Y"));
        keycodes.Add(("z","Z"));
        keycodes.Add(("1","!"));
        keycodes.Add(("2","@"));
        keycodes.Add(("3","#"));
        keycodes.Add(("4","$"));
        keycodes.Add(("5","%"));
        keycodes.Add(("6","^"));
        keycodes.Add(("7","&"));
        keycodes.Add(("8","*"));
        keycodes.Add(("9","("));
        keycodes.Add(("0",")"));
        keycodes.Add(("`","~"));
        keycodes.Add(("-","_"));
        keycodes.Add(("=","+"));
        keycodes.Add(("[","{"));
        keycodes.Add(("]","}"));
        keycodes.Add(("\\","|"));
        keycodes.Add((";",":"));
        keycodes.Add(("'","\""));
        keycodes.Add((",","<"));
        keycodes.Add((".",">"));
        keycodes.Add(("/","?"));
        keycodes.Add(("space","space"));
        keycodes.Add(("backspace","backspace"));
        keycodes.Add(("return","return"));
    }

    // Update is called once per frame
    private bool upperCase = false;
    private bool capsLock = false;
    private string output;
    void Update()
    {
        // Display boot screen
        if (!booted) {
            // Skip boot screen
            if (Input.GetKeyDown("return")) {
                booted = true;
                StopCoroutine(BootScreen());
                terminalText.alignment = TextAlignmentOptions.TopLeft;
            }
            if (!booting) {
                StartCoroutine(BootScreen());
            }
            return;
        }


        // if (ansiMode) {
        //     ansiApp.ANSIExecute(out output);
        //     // Custom input handling



        //     // Draw program output
        //     terminalText.text = output;

        //     return;
        // }
        // ansiApp = null;

        // Macros (I guess right?)
        if (Input.GetKeyDown(KeyCode.LeftShift) || 
            Input.GetKeyDown(KeyCode.RightShift)) {
            upperCase = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift) || 
            Input.GetKeyUp(KeyCode.RightShift)) {
            upperCase = false;
        }

        if (Input.GetKeyDown(KeyCode.CapsLock)) {
            if (!capsLock) {
                upperCase = true;
            } else {
                upperCase = false;
            }

            capsLock = !capsLock;
        }

        // Navigate command history with up/down
        // (with some fancy clamped (in|dec)rements)
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            cacheOffset = Mathf.Min(commandCache.Count,cacheOffset+1);
            cacheSet = false;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            cacheOffset = Mathf.Max(0,cacheOffset-1);
            cacheSet = false;

            // Exit viewing cache and return empty prompt
            if (cacheOffset == 0) {
                currentLine = prompt;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentCharOffset = Mathf.Min(terminalText.text.Length, currentCharOffset+1);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentCharOffset = Mathf.Max(0,currentCharOffset-1);
        }

        // Delete any non-visible lines (scrolling doesn't exist)
        if (previousLines.Count >= lineCount) {
            previousLines.RemoveRange(0,previousLines.Count-lineCount);
        }

        string nextInput = GetNextKBInput();
        switch (nextInput) {
            case "return":
                currentCharOffset = 0;
                previousLines.Add(currentLine);
                commandCache.Add(currentLine);
                cacheOffset = 0;
                cacheSet = false;
                output = ProcessCommands(currentLine);
                if (output == null) {
                    break;
                }
                if (output.Length > 0 && output[output.Length-1] != '\n') {
                    output += "\n";
                }
                currentLine = prompt;
                foreach(string outputLine in output.Split("\n")) {
                    previousLines.Add(outputLine);
                }
                break;

            case "backspace":
                currentCharOffset -= 1;
                if (currentLine.Length > prompt.Length) {
                    currentLine = currentLine.Substring(0,currentLine.Length-1);
                }
                break;

            case "space":
                currentCharOffset += 1;
                currentLine += " ";
                break;

            default:
                if (nextInput.Length > 0) {
                    currentCharOffset += 1;
                    currentLine += nextInput;
                }
                break;
        }

        // Reset screen
        terminalText.text = "";

        // Draw previous executed lines
        foreach (string line in previousLines) {
            terminalText.text += line + "\n";
        }

        // Restore previous command from history
        if (cacheOffset != 0 && !cacheSet) {
            // terminalText.text += commandCache[commandCache.Count-cacheOffset];
            string currentCacheLine = commandCache[commandCache.Count-cacheOffset];
            currentLine = currentCacheLine;
            // currentLine = currentCacheLine + (currentLine.Replace(prompt,"").Replace(currentCacheLine,""));
            cacheSet = true;
        }
        // Draw current input line
        terminalText.text += currentLine;

        // Blink cursor
        if (cursorBlink) {
            // terminalText.text += "_";
            // terminalText.text += "|";
            int cursorOffset = terminalText.text.Length - (currentLine.Length-currentCharOffset) + prompt.Length;
            try {
                terminalText.text = terminalText.text.Insert(cursorOffset,"_");
                // terminalText.text = terminalText.text.Insert(cursorOffset+1,"<u>");
                // terminalText.text = terminalText.text.Insert(cursorOffset+4,"</u>");
            } catch (System.Exception e) {
                Debug.Log(cursorOffset);
            }

        }        
        if (!blinkRunning) {
            StartCoroutine(CursorBlink());
        }
    }

    string GetNextKBInput() {
        for (int i=0; i<keycodes.Count; i++) {
            if (Input.GetKeyDown(keycodes[i].Item1)) {
                if (upperCase)
                    return keycodes[i].Item2;
                return keycodes[i].Item1;
            }
        }
        return "";
    }


    string ProcessCommands(string command) {
        command = command.Substring(prompt.Length);
        if (command.Length == 0) { return null; }

        string output = "";
        string[] args = command.Split(" ");
        switch (args[0].ToLower()) {
            case "clear":
            case "cls":
                Clear();
                return null;

            case "echo":
                if (args.Length < 2) {
                    return "Usage: echo [message]";
                }
                for (int i=1; i<args.Length; i++) {
                    if (i == args.Length) {
                        output += args[i];
                        break;
                    }
                    output += args[i] + " ";
                }
                return output;

            case "ping":
                if (args.Length < 2 || args.Length > 3) {
                    return "Usage: ping ip [packet_count]";
                }
                int packetCount = 4;
                if (args.Length == 3) { packetCount = int.Parse(args[2]); }
                if (packetCount > 20) {
                    return Error("Maximum packet count is 20");
                }
                if (!currentHost.Ping(args[1],packetCount,out output)) {
                    return Error(output);
                }
                return output;

            case "whoami":
            case "ipconfig":
                if (args.Length > 1) {
                    return "Usage: ipconfig";
                }

                if (!currentHost.IpConfig(out output)) {
                    return Error(output);
                }

                return output;

            case "ls":
                if (args.Length > 2) {
                    return "Usage: ls [pattern]";
                }
                if (!currentHost.ListPrograms(out output)) {
                    return Error(output);
                }
                return output;

            case "run":
                if (args.Length != 2) {
                    return "Usage: run filename";
                }
                if (!currentHost.RunProgram(args[1],out output)) {
                    return Error(output);
                }
                return output;

            case "connect":
                if (args.Length > 3 || args.Length < 2) {
                    return "Usage: connect ip [password]";
                }

                string password = "";
                if (args.Length == 3) {
                    password = args[2];
                }
                Computer transitionHost;
                if (!currentHost.ConnectTo(args[1], password, this, out transitionHost, out output)) {
                    return Error(output);
                }

                // previousHost = currentHost; // Store previous connection
                previousHosts.Add(currentHost);
                currentHost = transitionHost; // Update current connection
                // prompt = currentHost.HostName+">";
                prompt = GeneratePrompt(currentHost);
                return output;

            case "dc":
            case "disconnect":
                if (args.Length > 1) {
                    return "Usage: disconnect";
                }

                if (previousHosts.Count < 1) {
                    return Error("Not connected to anything");
                }

                Computer previousHost = previousHosts[previousHosts.Count-1];
                output = string.Format("Disconnected from {0}",currentHost.GetHostName());
                if (previousHost != baseHost) {
                    currentHost = previousHost;
                    // prompt = currentHost.HostName+">";
                    prompt = GeneratePrompt(currentHost);
                } else {
                    currentHost = baseHost;
                    // prompt = "$>";
                    prompt = GeneratePrompt(null);
                }
                previousHosts.RemoveAt(previousHosts.Count-1);

                /*if (previousHost != baseHost) {
                    currentHost = previousHost;
                } else {
                    currentHost = baseHost;
                }*/
                return output;

            case "netscan":
                if (args.Length > 1) {
                    return "Usage: netscan";
                }
                if (!currentHost.NetScan(out output)) {
                    return Error(output);
                }
                return output;

            case "reboot":
                if (currentHost == baseHost) {
                    GetComponent<TerminalManager>().ResetTerminal();
                } else {
                    currentHost.Reboot();
                }
                return "";

            // TODO: Find a better way to index & handle commands and,
            // afterwards, a better way to grab a list of all command words
            case "help":
                if (args.Length == 2) {
                    // Check for command name in args[1] and give specific help message
                }

                output = "clear/cls\necho\nping\nwhoami/ipconfig\nhelp\nrun\nconnect\ndisconnect/dc\nnetscan\nls\nreboot";
                return output;
        }
        return Error("Invalid command");
    }

    void Clear() {
        previousLines = new List<string>();
        currentLine = prompt;
    }

    string Error(string msg) {
        return "<color=red>[ERROR]:</color> "+msg;
    }

    string GeneratePrompt(Computer host) {
        if (host == null) {
            return "$>";
        }

        if (host.HostName.Length == 0) {
            return host.Ip+">";
        }

        return host.HostName+">";
    }

    float blinkDelay = 0.6f;
    bool cursorBlink = false;
    bool blinkRunning = false;
    IEnumerator CursorBlink() {
        blinkRunning = true;
        cursorBlink = true;
        yield return new WaitForSeconds(blinkDelay);
        cursorBlink = false;
        yield return new WaitForSeconds(blinkDelay);
        blinkRunning = false;
    }

    float bannerLineDrawDelay = 0.07f;
    float statusLineDrawDelay = 0.5f;
    float screenPausePeriod = 2.5f;
    //string bannerLines = "<color=yellow>                                OOOOO      SSSSSSS \n oooo   cccc ttttt        OOO   OOO  SSSSS     \n  oo  oo cc      t   -----  OOO   OOO    SSSSS    \n     oo  oo cc      t   -----  OOO   OOO       SSSSS \n    oooo   cccc   t            OOOOO    SSSSSSSS</color>";
    // string bannerLines = "<color=yellow>                                 OOOOO      SSSSSSS\n  oooo   cccc ttttt        OOO   OOO  SSSSS\n   oo  oo cc      t    -----   OOO   OOO    SSSSS\n      oo  oo cc      t   -----   OOO   OOO       SSSSS\n     oooo   cccc   t            OOOOO    SSSSSSSS</color>";
    // string bannerLines = "<color=yellow>TTTTTT EEEEEE RRRRR  MMM MMM EEEEEE XX  XX\n  TT   EE     RR  RR MM M MM EE     XX  XX\n TT   EEEE   RRRRR  MM M MM EEEEEE  XXXX\n  TT   EE     RR  RR MM   MM EE     XX  XX\n  TT   EEEEEE RR  RR MM   MM EEEEEE XX  XX</color>";
    string bannerLines = "<color=red>                         OOOOO      SSSSSSS \noooo   cccc ttttt  OOO   OOO  SSSSS     \n oo  oo cc      t    OOO   OOO    SSSSS   \n    oo  oo cc      t    OOO   OOO       SSSSS\n   oooo   cccc   t      OOOOO    SSSSSSSS</color>";
    string statusLines = "OBIOS(c)1996 \n<color=yellow>BIOS Build :</color> 0xEA2F\n<color=yellow>CPU :</color> Dual-Core GRF Omnitron(tm) Processor 280\n <color=yellow>Speed :</color> 1.80 GHz    <color=yellow>Count :</color> 4\n<color=yellow>Board Serial Number :</color> 100BD-122A092D44E17\n\nInitializing disks \t\t.. <color=green>Done</color>\nNegotiating circulatory link \t.. <color=green>Done</color>\n\n502576MB RAM OK";
    IEnumerator BootScreen() {
        booting = true;
        terminalText.alignment = TextAlignmentOptions.Center;
        terminalText.text = "";
        foreach (string line in bannerLines.Split('\n')) {
            terminalText.text += line + "\n";
            yield return new WaitForSeconds(bannerLineDrawDelay);
        }

        terminalText.text += "\n";

        foreach (string line in statusLines.Split('\n')) {
            terminalText.text += line + "\n";
            // Randomly offset line draw delay for The Aesthetic(tm)
            float randomOffset = Random.Range(-statusLineDrawDelay,statusLineDrawDelay*0.75f);
            yield return new WaitForSeconds(statusLineDrawDelay+randomOffset);
        }

        yield return new WaitForSeconds(screenPausePeriod);
        booted = true;
        terminalText.alignment = TextAlignmentOptions.TopLeft;
    }
}
