using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFlowVisualizer {
    public class SimplePathGenerator : MonoBehaviour {
        public static SimplePathGenerator Instance;
        public GameObject[] ShotPrefabs;
        public GameObject   PathPrefab;
        public List<GameObject> PathObjectList;
        public List<SimplePath> PathList;

        public enum SHOT_TYPE {
            FLOW_RED, FLOW_GREEN, FLOW_BLUE,
            GAS_RED,GAS_GREEN,GAS_BLUE,
            ELEC_RED,ELEC_GREEN, ELEC_BLUE
        }

        public class SimplePath:MonoBehaviour {
            public Vector3 StartPos;
            public Vector3 EndPos;

            public void DeletePath() {
                SimplePathGenerator.Instance.DeletePath(this);
            }
        }

        public class SimpleShot : MonoBehaviour {
            public Vector3 StartPos;
            public Vector3 EndPos;

            public float Speed;
            public float threshold = 0;

            private float distance = 0;
            private float old_distance = 0;

            public void Start() {
                this.transform.LookAt(EndPos);
                threshold = ( ( Speed / 4 ) <= 0.3f ) ? 0.3f : Speed / 4;
                old_distance = Vector3.Distance(this.transform.position, EndPos);
            }
            public void FixedUpdate() {
                this.transform.position += this.transform.forward * Speed;
                distance = Vector3.Distance(this.transform.position, EndPos);
                if (Vector3.Distance(this.transform.position,EndPos) < threshold)
                    Destroy(this.gameObject);
                if (old_distance < distance)
                    Destroy(this.gameObject);
                else old_distance = distance;
            }
        }

        public void Awake() {
            Instance = this;
            PathObjectList = new List<GameObject>();
            PathList = new List<SimplePath>();
        }

        public GameObject InstantiatePath(Transform Start, Transform End, Color PathColor, float Thickness = 1f) {
            GameObject returnObject = Instantiate(PathPrefab);
            returnObject.transform.position = Start.position;
            returnObject.transform.rotation = Quaternion.identity;
            returnObject.transform.LookAt(End.position);
            returnObject.transform.localScale = new Vector3(Thickness, Thickness, Vector3.Distance(Start.position, End.position));
            MeshRenderer mesh = returnObject.GetComponent<MeshRenderer>();
            mesh.materials[0].color = PathColor;
            PathObjectList.Add(returnObject);
            SimplePath newPath = returnObject.AddComponent<SimplePath>();
            newPath.StartPos = Start.position; newPath.EndPos = End.position;
            PathList.Add(newPath);
            return returnObject;
        }

        public GameObject InstantiatePath(Vector3 Start, Vector3 End, Color PathColor, float Thickness = 1f) {
            GameObject returnObject = Instantiate(PathPrefab);
            returnObject.transform.position = Start;
            returnObject.transform.rotation = Quaternion.identity;
            returnObject.transform.LookAt(End);
            returnObject.transform.localScale = new Vector3(Thickness, Thickness, Vector3.Distance(Start, End));
            MeshRenderer mesh = returnObject.GetComponent<MeshRenderer>();
            mesh.materials[0].color = PathColor;
            PathObjectList.Add(returnObject);
            SimplePath newPath = returnObject.AddComponent<SimplePath>();
            newPath.StartPos = Start; newPath.EndPos = End;
            PathList.Add(newPath);
            return returnObject;
        }

        public GameObject InstantiatePath(Transform Parent, Transform Start, Transform End, Color PathColor, float Thickness = 1f) {
            GameObject returnObject = Instantiate(PathPrefab);
            returnObject.transform.position = Start.position;
            returnObject.transform.rotation = Quaternion.identity;
            returnObject.transform.LookAt(End.position);
            returnObject.transform.localScale = new Vector3(Thickness, Thickness, Vector3.Distance(Start.position, End.position));
            returnObject.transform.parent = Parent;
            MeshRenderer mesh = returnObject.GetComponent<MeshRenderer>();
            mesh.materials[0].color = PathColor;
            PathObjectList.Add(returnObject);
            SimplePath newPath = returnObject.AddComponent<SimplePath>();
            newPath.StartPos = Start.position; newPath.EndPos = End.position;
            PathList.Add(newPath);
            return returnObject;
        }

        public GameObject InstantiatePath(Transform Parent, Vector3 Start, Vector3 End, Color PathColor, float Thickness = 1f) {
            GameObject returnObject = Instantiate(PathPrefab);
            returnObject.transform.position = Start;
            returnObject.transform.rotation = Quaternion.identity;
            returnObject.transform.LookAt(End);
            returnObject.transform.localScale = new Vector3(Thickness, Thickness, Vector3.Distance(Start, End));
            returnObject.transform.parent = Parent;
            MeshRenderer mesh = returnObject.GetComponent<MeshRenderer>();
            mesh.materials[0].color = PathColor;
            PathObjectList.Add(returnObject);
            SimplePath newPath = returnObject.AddComponent<SimplePath>();
            newPath.StartPos = Start; newPath.EndPos = End;
            PathList.Add(newPath);
            return returnObject;
        }

        public void DeletePath(SimplePath path) {
            GameObject targetOb = null;
            int targetID = path.GetInstanceID();
            int index = -1;
            for(int i = 0; i < PathList.Count; i++) {
                if(PathList[i].GetInstanceID() == targetID) {
                    targetOb = PathObjectList[i];
                    index = i;
                    break;
                }
            }

            if(targetOb != null) {
                PathObjectList.RemoveAt(index);
                PathList.RemoveAt(index);
                Destroy(targetOb);
            }
        }

        public void ClearPath() {
            for(int  i = 0; i < PathObjectList.Count; i++) {
                GameObject go = PathObjectList[i];
                Destroy(go);
            }
            PathObjectList.Clear();
        }
    
        public GameObject Shooting(SimplePath path, float speed = 0.2f, SHOT_TYPE type = SHOT_TYPE.FLOW_BLUE) {
            GameObject returnObject = Instantiate(ShotPrefabs[(int)type], path.StartPos,Quaternion.identity);
            SimpleShot shot = returnObject.AddComponent<SimpleShot>();
            shot.StartPos = path.StartPos;
            shot.EndPos = path.EndPos;
            shot.Speed = speed;
            return returnObject;
        }

        public GameObject Shooting(Transform start, Transform end, float speed = 0.2f, SHOT_TYPE type = SHOT_TYPE.FLOW_BLUE) {
            GameObject returnObject = Instantiate(ShotPrefabs[(int)type], start.position, Quaternion.identity);
            SimpleShot shot = returnObject.AddComponent<SimpleShot>();
            shot.StartPos = start.position;
            shot.EndPos = end.position;
            shot.Speed = speed;
            return returnObject;
        }

        public GameObject Shooting(Vector3 start, Vector3 end, float speed = 0.2f, SHOT_TYPE type = SHOT_TYPE.FLOW_BLUE) {
            GameObject returnObject = Instantiate(ShotPrefabs[(int)type], start, Quaternion.identity);
            SimpleShot shot = returnObject.AddComponent<SimpleShot>();
            shot.StartPos = start;
            shot.EndPos = end;
            shot.Speed = speed;
            return returnObject;
        }
    }

}

