using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    // Communicate to other computers
    // Networking
    // Directory structure
    // Device access (open doors through pc, etc)

    public bool isMyPC = false;

    public Router router; // Manually assigned
    private string mac;
    public string Mac {
        get { return mac; }
    }
    private string ip;
    public string Ip {
        get { return ip; }
    }

    private Terminal terminalSession;
    public Terminal TerminalSession {
        get { return terminalSession; }
        set {
            terminalSession = value;
        }
    }

    private List<Program> programs;

    private string hostName = "";
    public string HostName {
        get { return hostName; }
    }
    private string password = "password";

    // Start is called before the first frame update
    void Start()
    {
        mac = "12:34:56:78:90:AB";
        ip = router.RegisterDynamicIP(this);

        programs = new List<Program>();
        if (isMyPC) {
            programs.Add(gameObject.AddComponent<Program>());
            programs.Add(gameObject.AddComponent<ANSIApp>());
            programs.Add(gameObject.AddComponent<PingTest>());
        } else {
            programs.Add(gameObject.AddComponent<Program>());
            programs.Add(gameObject.AddComponent<Program>());
            programs.Add(gameObject.AddComponent<Program>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ip == null || ip == "") {
            StartCoroutine(AttemptRouterConnect());
        }
    }

    public void Reboot() {
        Debug.Log("Rebooting...");
    }

    public bool Ping(string destination, int packetCount, out string output) {
        output = "";
        if (AddressIsLocalHost(destination)) {
            for (int i=0; i<packetCount; i++) {
                output += string.Format("Response from {0}\n","127.0.0.1");
            }
            return true;
        }

        try {
            if (router.gameObject) {}
        } catch (Exception e) {
            output = "Router unreachable\n";
            return false;
        }

        for (int i=0; i<packetCount; i++) {
            if (!router.Ping(destination)) {
                output += "Destination unreachable\n";
                /*return false;*/
                continue;
            }
            output += string.Format("Response from {0}\n",destination);
        }

        return true;
    }

    public bool IpConfig(out string output) {
        if (ip == null) {
            output = "Disconnected from network";
            return false;
        }

        output = string.Format("IP: {0}\nMAC: {1}",ip,mac);
        return true;
    }


    public bool RunProgram(string fileName, out string output) {
        output = "";
        foreach (Program p in programs) {
            if (p.fileName == fileName) {

                // Check for ANSI App
                if (p.GetType() == typeof(ANSIApp)) {
                    terminalSession.ansiApp = (ANSIApp)p;
                }

                p.Execute(this,out output);
                return true;
            }
        }

        output = "No such file exists";
        return false;
    }

    public bool ListPrograms(out string output) {
        output = "";
        for (int i=0; i<programs.Count; i++) {
            output += programs[i].fileName;
            if (i<programs.Count-1) { output += "\n"; }
        }
        return true;
    }

    /*private Computer connectedTo;
    public Computer ConnectedTo {
        get { return connectedTo; }
    }*/
    // private string connectedTo;
    public bool ConnectTo(string destination, string password, Terminal session, out Computer newHost, out string output) {

        newHost = null;
        if (AddressIsLocalHost(destination)) {
            // output = "Cannot nest connections to local host";
            output = "Already connected";
            return false;
        }
        if (!router.ConnectTo(destination, password, session, out newHost, out output)) {
            return false;
        }
        // connectedTo = destination;
        return true;
    }

    public bool Connect(Terminal session, string password, out string output) {
        if (password == this.password || this.password == "") {
            terminalSession = session;
            string name = GetHostName();
            output = string.Format("Authenticated to {0}",name);
            return true;
        }
        output = "Failed to authenticate";
        return false;
    }

    public bool NetScan(out string output) {
        return router.GetDevices(out output);
    }

    public bool SetHostName(string hostName) {
        if (hostName.Length == 0) {
            return false;
        }
        this.hostName = hostName;
        return true;
    }

    public string GetHostName() {
        if (hostName.Length == 0)
            return ip;
        return hostName;
    }

    private bool AddressIsLocalHost(string address) {
        return (address == "127.0.0.1" || address == "localhost" || address == this.ip || address == this.hostName);
    }

    private float connectionRetryPeriod = 5f;
    IEnumerator AttemptRouterConnect() {
        ip = router.RegisterDynamicIP(this);
        yield return new WaitForSeconds(connectionRetryPeriod);
    }

}
