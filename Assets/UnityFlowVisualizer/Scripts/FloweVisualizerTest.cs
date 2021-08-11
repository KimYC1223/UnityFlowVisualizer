using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class FloweVisualizerTest : MonoBehaviour
    {
        public GameObject flowObject_Red;
        public GameObject flowObject_Blue;
        public GameObject flowObject_Green;
        public void ButtonClick_Blue() {
            PathGroup pathGroup = PathManager.Instance.PathGroupList[0];
            Path loopPath = pathGroup.PathList[0];

            FlowEngine shot = FlowEngine.Shoot(flowObject_Blue, this.transform, loopPath, 0.4f,1f,0.2f);
        }

        public void ButtonClick_Red1() {
            PathGroup pathGroup = PathManager.Instance.PathGroupList[1];
            Path loopPath = pathGroup.PathList[0];

            FlowEngine shot = FlowEngine.Shoot(flowObject_Red, this.transform, loopPath, 0.4f, 1f, 0.2f);
        }
        public void ButtonClick_Red2() {
            PathGroup pathGroup = PathManager.Instance.PathGroupList[1];
            Path loopPath = pathGroup.PathList[1];

            FlowEngine shot = FlowEngine.Shoot(flowObject_Red, this.transform, loopPath, 0.4f, 1f, 0.2f);
        }

        public void ButtonClick_Green1() {
            PathGroup pathGroup = PathManager.Instance.PathGroupList[2];
            Path loopPath = pathGroup.PathList[0];

            FlowEngine shot = FlowEngine.Shoot(flowObject_Green, this.transform, loopPath, 0.4f, 1f, 0.2f);
        }

        public void ButtonClick_Green2() {
            PathGroup pathGroup = PathManager.Instance.PathGroupList[2];
            Path loopPath = pathGroup.PathList[0];

            FlowEngine shot = FlowEngine.Shoot(flowObject_Green, this.transform, loopPath, 0.4f, 1f, 0.2f);
            shot.Callback += ButtonClick_Red1;
        }
    }
}
