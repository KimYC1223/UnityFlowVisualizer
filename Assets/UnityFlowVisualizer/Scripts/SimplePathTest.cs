using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFlowVisualizer;

public class SimplePathTest : MonoBehaviour
{
    public Transform StartTr;
    public Transform EndTr;
    public Color PathColor;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(SimplePathGenerator.Instance == null);
        SimplePathGenerator.Instance.InstantiatePath(StartTr, EndTr, PathColor, 1f);
        StartCoroutine(Shooting());
    }

    // Update is called once per frame
    IEnumerator Shooting()
    {
        while(true) {
            SimplePathGenerator.Instance.Shooting(StartTr, EndTr);
            yield return new WaitForSeconds(0.35f);
        }
    }
}
