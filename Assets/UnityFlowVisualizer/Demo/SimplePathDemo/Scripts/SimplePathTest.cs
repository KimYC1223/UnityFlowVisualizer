using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class SimplePathTest : MonoBehaviour
    {
        public Transform StartTr;
        public Transform EndTr;
        public Color PathColor;

        // Start is called before the first frame update
        void Start()
        {
            SimplePathGenerator.Instance.InstantiatePath(StartTr, EndTr, PathColor, 1f);
            StartCoroutine(Shooting());
        }

        // Update is called once per frame
        IEnumerator Shooting()
        {
            while(true) {
                SimplePathGenerator.Instance.Shooting(StartTr, EndTr,1,2f);
                yield return new WaitForSeconds(0.35f);
            }
        }
    }
}
