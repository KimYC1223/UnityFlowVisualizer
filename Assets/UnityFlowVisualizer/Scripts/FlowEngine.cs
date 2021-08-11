using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityFlowVisualizer {

    public class FlowEngine : MonoBehaviour
    {
        public static FlowEngine Shoot(GameObject flowObject, Transform parents, Path path, float speed, float size, float sensitivity = 0.5f ) {
            GameObject shot = Instantiate(flowObject);
            FlowEngine engine = shot.AddComponent<FlowEngine>();
            engine.transform.localScale = new Vector3(size,size,size);
            engine.transform.parent = parents;
            engine.Speed = speed;
            engine.FollowingPath = path;
            engine.minimunDistance = speed * sensitivity;

            engine.CurrentConnectionIndex = 0;
            engine.CurrentPointerIndex = 0;
            engine.MaxPointerIndex = engine.FollowingPath.Connections[0].CornerCount + 1;
            engine.MaxConnectionIndex = engine.FollowingPath.Connections.Count-1;
            if (engine.FollowingPath.Connections[0].CornerList.Count == 0)
                engine.nextTarget = engine.FollowingPath.Connections[0].EndPos;
            else engine.nextTarget = engine.FollowingPath.Connections[0].CornerList[0].transform;
            engine.transform.position = engine.FollowingPath.Connections[0].StartPos.position;

            return engine;
        }

        public float Speed;
        public Path FollowingPath;
        public bool isLoopFlow = false;
        [Min (0.001f)]
        public static float sensitivity = 0.5f;
            
        private int CurrentConnectionIndex = 0;
        private int MaxConnectionIndex = 0;
        private int CurrentPointerIndex = 0;
        private int MaxPointerIndex = 0;
        private float minimunDistance = 0f;
        private Transform nextTarget = null;

        public delegate void CallbackFunction();
        public CallbackFunction Callback;

        public void FixedUpdate() {
            try {
                if (isLoopFlow && ( FollowingPath.StartID != FollowingPath.EndID ))
                    DestroyImmediate(this.gameObject);

                if (nextTarget != null) {
                    float distance = GetDistacne();
                    if (distance == -1) DestroyImmediate(this.gameObject);
                    else if (distance < sensitivity) {
                        this.transform.position = nextTarget.position;
                        nextTarget = SetNextTarget();
                        if (nextTarget == null) DestroyImmediate(this.gameObject);
                    } else {
                        this.transform.LookAt(nextTarget);
                        this.transform.position += Speed * this.transform.forward;
                    }
                } else {
                    nextTarget = SetNextTarget();
                    if (nextTarget == null) DestroyImmediate(this.gameObject);
                }
            } catch (System.Exception e) {
                e.ToString(); DestroyImmediate(this.gameObject);
            }
        }

        public Transform SetNextTarget() {
            Transform nextTarget = null;
            List<Connection> cons = FollowingPath.Connections;
            CurrentPointerIndex++;
            if(CurrentPointerIndex == MaxPointerIndex) {

                if(CurrentConnectionIndex == MaxConnectionIndex) {
                    if(Callback != null)
                        Callback();
                    if(isLoopFlow) {
                        CurrentConnectionIndex = 0;
                        CurrentPointerIndex = 0;
                        if (cons[0].CornerList.Count == 0) {
                            MaxPointerIndex = 1;
                            nextTarget = cons[0].EndPos;
                        } else {
                            MaxPointerIndex = cons[0].CornerList.Count + 1;
                            nextTarget = cons[0].CornerList[0].transform;
                        }
                    } else nextTarget = null;
                } else {
                    CurrentConnectionIndex++;
                    CurrentPointerIndex = 0;
                    MaxPointerIndex = cons[CurrentConnectionIndex].CornerList.Count + 1;
                    if(cons[CurrentConnectionIndex].CornerList.Count == 0)
                         nextTarget = cons[CurrentConnectionIndex].EndPos;
                    else nextTarget = cons[CurrentConnectionIndex].CornerList[0].transform;
                }
            } else {
                if(CurrentPointerIndex == cons[CurrentConnectionIndex].CornerList.Count)
                    nextTarget = cons[CurrentConnectionIndex].EndPos; 
                else nextTarget = cons[CurrentConnectionIndex].CornerList[CurrentPointerIndex].transform;
            }

            return nextTarget;
        }


        public float GetDistacne() {
            if(nextTarget != null) 
                return Vector3.Distance(this.transform.position, nextTarget.position);
            else return -1;
        }
    }
}
