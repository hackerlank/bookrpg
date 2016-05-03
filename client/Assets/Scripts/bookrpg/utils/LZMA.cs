using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System;

public class LZMA : MonoBehaviour {

    [DllImport ("bookrpg")]
    private static extern void myStrncpy (
        [MarshalAs(UnmanagedType.LPWStr)]StringBuilder dest, 
        [MarshalAs(UnmanagedType.LPWStr)]string src, uint n);


    [DllImport ("bookrpg")]
    private static extern int SubNum ([In, Out]int a, int b);

    [DllImport ("bookrpg")]
    private static extern int myStr ([In] int[] arr);

    public Text txt;

    void Start()
    {

        int[] arr = new int[23];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = i+1;
        }
        myStr(arr);
        Debug.Log(arr[0]);

        int a = 5;

        SubNum(a, 1);

        Debug.Log(a);

       
    }
}
