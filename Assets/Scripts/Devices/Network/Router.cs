using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Router : MonoBehaviour
{
    // All subnets go from .0 - .255 (.0 & .255 are invalid, and .1 is the router)

    // private List<(Computer,string)> devices; // All connected devices (PC,ip)
    private (Computer,string,string)[] devices; // (PC,ip,mac)
    private string subnet = "172.168.0";
    public string Subnet {
        get { return subnet; }
    }
    private string ip;
    public string Ip {
        get { return ip; }
    }

    private void Start() {
        ip = subnet+".1";
        devices = new (Computer,string,string)[255];
    }

    // PC attempts to connect and is given a valid IP.
    // (null means there were no unpopulated addresses)
    public string RegisterDynamicIP(Computer requester) {
        try {
            if (devices.Length > 0) {}
        } catch (Exception e) {
            return null;
        }

        string newIP = null;

        // Register first available address.
        for (int i=0; i<devices.Length; i++) {
            if (i == 0 || i == 1 || i == 255) {
                continue;
            }

            if (devices[i].Item1 == null) {
                newIP = subnet+"."+i.ToString();
                devices[i] = (requester,newIP,requester.Mac);
                break;
            }
        }

        return newIP;
    }

    // PC attempts to connect with a desired IP.
    // (false means the IP was taken or invalid
    // or there were no unpopulated addresses)
    public bool RegisterStaticIP(Computer requester, string ip) {
        string[] octets = ip.Split(".");
        string requestedSubnet = octets[0] + "." + octets[1] + "." + octets[2];
        int host = int.Parse(octets[3]);

        if (host==0 || host==1 || host==255) { return false; }
        if (devices[host].Item1 == null) {
            devices[host] = (requester,ip,requester.Mac);
            return true;
        }
        /*for (int i=0; i<devices.Length; i++) {
            if (i==0 || i==1 || i==255) {
                continue;
            }

            if (devices[i].Item1 == ip) {
                return false;
            }
        }*/

        return false;
    }

    // PC attempts to disconnect and forfeit
    // their allocated IP.
    // (false means unregistering failed or
    // the IP was never registered to begin with)
    public bool UnregisterIP(string ip) {
        bool status = false;

        return status;
    }


    // Super basic (fake) implementation of ping.
    // To improve, work on Computer.RespondToPing()
    // but this should be sufficient for now
    
    // Computer lastPingDest = null;
    string lastPingCache = null;
    public bool Ping(string destination) {
        if (lastPingCache == destination) {
            return true;
        }

        // Pinging the router
        if (destination == ip) {
            return true;
        }

        // Pinging an endpoint
        for (int i=0; i<devices.Length; i++) {
            if (devices[i].Item1 == null) { continue; }
            if (devices[i].Item2 == destination || devices[i].Item1.HostName == destination) {
                lastPingCache = devices[i].Item2;
                return true;
            }
        }

        return false;
    }

    public bool ConnectTo(string destination, string password, Terminal session, out Computer newHost, out string output) {
        newHost = null;
        for (int i=0; i<devices.Length; i++) {
            if (devices[i].Item1 == null) { continue; }
            if (devices[i].Item2 == destination || devices[i].Item1.HostName == destination) {
                if (!devices[i].Item1.Connect(session, password, out output)) {
                    return false;
                }
                Debug.Log("Setting newhost to: "+devices[i].Item2);
                newHost = devices[i].Item1;
                return true;
            }
        }
        output = "Host does not exist";
        return false;
    }

    public bool GetDevices(out string output) {
        output = "";
        for (int i=0; i<devices.Length; i++) {
            if (devices[i].Item1 != null) {

                if (devices[i].Item1.HostName != null && devices[i].Item1.HostName != "") {
                    output += devices[i].Item1.HostName + " : " + devices[i].Item2;
                } else {
                    output += devices[i].Item2;
                }
                if (i < devices.Length-1) {
                    output += "\n";
                }
            }
        }
        return true;
    }
}
