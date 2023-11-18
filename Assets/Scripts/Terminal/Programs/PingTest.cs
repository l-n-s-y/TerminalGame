using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingTest : Program
{
    public void Start() {
        this.fileName = "ping_test";
    }

    public override bool Execute(Computer host, out string output) {
        if (!host.Ping(host.router.Ip, 4, out output)) {
            output = "PingTest failed";
            return false;
        }
        return true;
    }
}
