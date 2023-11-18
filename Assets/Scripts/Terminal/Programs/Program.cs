using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    // Generate a random filename
    public string fileName;

    void Start() {
        fileName = "program_" + Random.Range(9999,99999).ToString();
    }

    public virtual bool Execute(Computer host, out string output) {
        output = "Hello World!";
        return true;
    }
}
