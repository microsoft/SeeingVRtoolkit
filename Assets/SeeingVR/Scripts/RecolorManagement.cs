// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RecolorManagement : MonoBehaviour
{
    public bool enableRecoloring = true;

    public bool dynamicScanning = false;
    private struct MaterialInfo
    {
        public GameObject currentObj;
        public int currentColor;
        public int priorColor;
        public int[] edges;
        public int edgeNum;
        public bool inView;
        public bool colored;


        public MaterialInfo(int color, int preColor, GameObject obj)
        {
            currentObj = obj;
            currentColor = color;
            priorColor = preColor;
            edges = new int[1000];
            edgeNum = 0;
            inView = true;
            colored = false;
        }

        public void setObj(GameObject obj)
        {
            currentObj = obj;
        }

        public void setColor(int color)
        {
            this.currentColor = color;
        }

        public void setPriorColor(int color)
        {
            priorColor = color;
        }

        public void clearEdge()
        {
            edgeNum = 0;
        }

        public void addEdge(int s)
        {
            edges[edgeNum] = s;
            edgeNum++;
        }
    }


    public float threshould = 0.1f;
    Color[] colors = { Color.green, Color.red, Color.blue, Color.yellow, Color.cyan, Color.magenta, new Color32(128, 0, 128, 255), Color.white};
    string[] colorNames = { "green", "red", "blue", "yellow", "cyan", "magenta" };
    private int colorNum = 8;
    Material newMat;
    Dictionary<int, MaterialInfo> materialMap = new Dictionary<int, MaterialInfo>();
    Dictionary<int, Material[]> originalMaterialMap = new Dictionary<int, Material[]>();
    bool prior_colorStatus = true;

    void Start()
    {

        newMat = new Material(Shader.Find("Specular"));
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.activeInHierarchy)
                {
                    if (obj.isWholeObject() && !isSubObject(obj.transform))
                    {
                        MaterialInfo info = new MaterialInfo(-1, -1, obj);
                        materialMap.Add(obj.GetInstanceID(), info);
                        changeMaterial(obj.transform);
                    }

                }
            }

    }

    bool isSubObject(Transform obj)
    {
        Transform parent = obj.parent;
        while (parent != null)
        {
            if (parent.gameObject.isWholeObject())
            {
                return true;
            }

            parent = parent.parent;
        }

        return false;
    }

    void recover()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            int instanceID = obj.GetInstanceID();
            if (originalMaterialMap.ContainsKey(instanceID))
            {
                obj.GetComponent<Renderer>().materials = originalMaterialMap[instanceID];
            }
        }
    }

    void Update()
    {
        if (!enableRecoloring && prior_colorStatus)
        {
            recover();
            prior_colorStatus = enableRecoloring;
            return;
        }

        if (!enableRecoloring) return;

        if (enableRecoloring && !prior_colorStatus)
        {
            foreach (var key in materialMap.Keys)
            {
                changeMaterial(materialMap[key].currentObj.transform);
            }

            prior_colorStatus = enableRecoloring;
        }

        if (dynamicScanning)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {

                if (obj.activeInHierarchy)
                {
                    if (obj.isWholeObject() && !isSubObject(obj.transform))
                    {

                        int instanceID = obj.GetInstanceID();
                        if (!materialMap.ContainsKey(instanceID))
                        {
                            MaterialInfo info = new MaterialInfo(-1, -1, obj);
                            materialMap.Add(instanceID, info);
                            changeMaterial(obj.transform);
                        }

                    }

                }

            }
        }



        List<int> list = new List<int>(materialMap.Keys);
        if (list.Count == 0)
            return;

        for (int i = 0; i<list.Count; i++)
        {
            var info = materialMap[list[i]];
            info.priorColor = info.currentColor;
            info.currentColor = -1;

            info.edgeNum = 0;
            Bounds bound = GetBounds(info.currentObj.transform);
            Rect rect = GetRectinCameraView(bound, 0);
            if (!WhetherInCameraView(rect))
            {
                info.inView = false;
            }
            else if (!info.currentObj.activeSelf || !info.currentObj.activeInHierarchy)
            {
                info.inView = false;
            }
            else if (occludedbound(bound))
            {
                info.inView = false;
            }
            else
            {
                info.inView = true;
            }
            info.colored = false;
            materialMap[list[i]] = info;
        }



        for (int i = 0; i<list.Count; i++)
        {
            if (!materialMap[list[i]].inView) continue;

            Bounds bound1 = GetBounds(materialMap[list[i]].currentObj.transform);
            Rect rect1 = GetRectinCameraView(bound1, 0);

            for (int j = i + 1; j < list.Count; j++)
            {

                if (!materialMap[list[j]].inView) continue;

                Bounds bound2 = GetBounds(materialMap[list[j]].currentObj.transform);
                Rect rect2 = GetRectinCameraView(bound2, 0);

                if (bound2.Intersects(bound1))
                {
                    var info = materialMap[list[i]];
                    info.edges[info.edgeNum] = list[j];
                    info.edgeNum++;
                    materialMap[list[i]] = info;

                    info = materialMap[list[j]];
                    info.edges[info.edgeNum] = list[i];
                    info.edgeNum++;
                    materialMap[list[j]] = info;
                    continue;
                }
                Rect rect2_new = Rect.MinMaxRect(rect2.xMin - threshould, rect2.yMin - threshould, rect2.xMax + threshould, rect2.yMax + threshould);
                if (rect1.Overlaps(rect2_new))
                {
                    var info = materialMap[list[i]];
                    info.edges[info.edgeNum] = list[j];
                    info.edgeNum++;
                    materialMap[list[i]] = info;

                    info = materialMap[list[j]];
                    info.edges[info.edgeNum] = list[i];
                    info.edgeNum++;
                    materialMap[list[j]] = info;
                }

            }
        }


        HashSet<int> toColor = new HashSet<int>();
        for (int i = 0; i<list.Count; i++)
        {
            if (materialMap[list[i]].inView)
            {
                toColor.Add(list[i]);
            } else
            {
                var myinfo = materialMap[list[i]];
                if (!myinfo.inView)
                {
                    myinfo.currentColor = myinfo.priorColor;
                    myinfo.colored = true;
                    materialMap[list[i]] = myinfo;

                }
            }
        }

        Queue<int> queue = new Queue<int>();

        while (toColor.Count != 0)
        {
            IEnumerator<int> ie = toColor.GetEnumerator();
            if (ie.MoveNext())
            {
                int obj = ie.Current;
                queue.Enqueue(obj);
            }

            while (queue.Count != 0)
            {
                int current = queue.Dequeue();

                var info = materialMap[current];

                int color = info.priorColor;


                bool[] whetherused = new bool[colorNum];
                for (int i = 0; i < colorNum; i++) whetherused[i] = false;
                for (int j = 0; j < info.edgeNum; j++)
                {
                    int edgeColor = materialMap[info.edges[j]].currentColor;
                    if (edgeColor != -1)
                    {
                        whetherused[edgeColor] = true;
                    } else
                    {
                    }
                }

                if (color != -1 && !whetherused[color])
                {
                    info.currentColor = color;
                }
                else
                {

                    for (int i = 0; i < colorNum; i++)
                    {
                        if (!whetherused[i])
                        {
                            info.currentColor = i;
                            break;
                        }
                    }

                }

                info.colored = true;
                materialMap[current] = info;

                toColor.Remove(current);

                for (int j = 0; j < info.edgeNum; j++)
                {
                    if (!materialMap[info.edges[j]].colored)
                    {
                        queue.Enqueue(info.edges[j]);
                    }
                }

            }
        }


        for (int i = 0; i<list.Count; i++)
        {
            if (!materialMap[list[i]].inView)
            {
                changeColor(materialMap[list[i]].currentObj.transform, Color.black);
            }
            else
            {
                int color = materialMap[list[i]].currentColor;
                if (color == -1)
                {
                    changeColor(materialMap[list[i]].currentObj.transform, Color.white);
                } else
                {
                    changeColor(materialMap[list[i]].currentObj.transform, colors[color]);
                }
            }

        }

    }

    bool HasRenderer(Transform obj)
    {
        if (obj.GetComponent<Renderer>() != null)
            return true;
        foreach (Transform child in obj)
        {
            if (HasRenderer(child))
                return true;
        }
        return false;
    }

    void changeMaterial(Transform obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && !renderer.gameObject.name.Equals("Guidline"))
        {

            if (renderer.materials != null && renderer.materials.Length != 0)
            {
                int instanceID = obj.gameObject.GetInstanceID();
                if (!originalMaterialMap.ContainsKey(instanceID))
                {
                    originalMaterialMap.Add(instanceID, renderer.materials);
                }
                else
                {
                    originalMaterialMap[instanceID] = renderer.materials;
                }
            }
            renderer.materials = new Material[renderer.materials.Length];

            for (int i=0; i<renderer.materials.Length;i++)
            {
                renderer.materials[i] = newMat;
            }

        }
        foreach (Transform child in obj)
        {
            changeMaterial(child);
        }
    }

    void changeColor(Transform obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && !renderer.gameObject.name.Equals("Guidline"))
        {
            for (int i=0;i<renderer.materials.Length;i++)
            {
                renderer.materials[i].color = color;
            }
        }
        foreach (Transform child in obj)
        {
            changeColor(child, color);
        }
    }

    // Colorable: not particle; not light; not teleport; not player or controller
    bool NotColorable(Transform obj)
    {
        if (obj.GetComponent<ParticleSystem>() != null || obj.GetComponent<Light>() != null
            || obj.GetComponent<TeleportPoint>() != null || obj.GetComponent<Teleport>() != null ||
            obj.GetComponent<Camera>() != null || obj.GetComponent<Hand>() != null ||obj.GetComponent<SteamVR_TrackedObject>() != null)
            return true;

        if (obj.childCount == 0) return false;

        bool colorable = true;
        foreach (Transform child in obj)
        {
            if (NotColorable(child))
            {
                colorable = false;
                break;
            }
        }
        if (!colorable) return true;
        return false;
    }

    Bounds GetBounds(Transform obj)
    {
        Bounds bounds;
        Renderer parentRender = obj.GetComponent<Renderer>();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (parentRender != null)
            bounds = parentRender.bounds;
        else
        {
            Vector3 center = Vector3.zero;

            foreach (Renderer render in renderers)
            {
                center += render.bounds.center;
            }
            center /= renderers.Length; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
            bounds = new Bounds(center, Vector3.zero);
        }


        foreach (Renderer render in renderers)
        {
            bounds.Encapsulate(render.bounds);
        }
        return bounds;
    }

    double BoundsDistance(Bounds boundx, Bounds boundy)
    {
        if (boundx.Intersects(boundy))
            return 0;
        Vector3 centerx = boundx.center;
        Vector3 extentx = boundx.extents;

        Vector3[] pointArrayX = new Vector3[8];
        pointArrayX[0] = new Vector3(centerx.x - extentx.x, centerx.y + extentx.y, centerx.z - extentx.z);  // Front top left corner
        pointArrayX[1] = new Vector3(centerx.x + extentx.x, centerx.y + extentx.y, centerx.z - extentx.z);  // Front top right corner
        pointArrayX[2] = new Vector3(centerx.x - extentx.x, centerx.y - extentx.y, centerx.z - extentx.z);  // Front bottom left corner
        pointArrayX[3] = new Vector3(centerx.x + extentx.x, centerx.y - extentx.y, centerx.z - extentx.z);  // Front bottom right corner
        pointArrayX[4] = new Vector3(centerx.x - extentx.x, centerx.y + extentx.y, centerx.z + extentx.z);  // Back top left corner
        pointArrayX[5] = new Vector3(centerx.x + extentx.x, centerx.y + extentx.y, centerx.z + extentx.z);  // Back top right corner
        pointArrayX[6] = new Vector3(centerx.x - extentx.x, centerx.y - extentx.y, centerx.z + extentx.z);  // Back bottom left corner
        pointArrayX[7] = new Vector3(centerx.x + extentx.x, centerx.y - extentx.y, centerx.z + extentx.z);  // Back bottom right corner

        Vector3[] pointArrayY = new Vector3[8];
        Vector3 centery = boundy.center;
        Vector3 extenty = boundy.extents;
        pointArrayY[0] = new Vector3(centery.x - extenty.x, centery.y + extenty.y, centery.z - extenty.z);
        pointArrayY[1] = new Vector3(centery.x + extenty.x, centery.y + extenty.y, centery.z - extenty.z);
        pointArrayY[2] = new Vector3(centery.x - extenty.x, centery.y - extenty.y, centery.z - extenty.z);
        pointArrayY[3] = new Vector3(centery.x + extenty.x, centery.y - extenty.y, centery.z - extenty.z);
        pointArrayY[4] = new Vector3(centery.x - extenty.x, centery.y + extenty.y, centery.z + extenty.z);
        pointArrayY[5] = new Vector3(centery.x + extenty.x, centery.y + extenty.y, centery.z + extenty.z);
        pointArrayY[6] = new Vector3(centery.x - extenty.x, centery.y - extenty.y, centery.z + extenty.z);
        pointArrayY[7] = new Vector3(centery.x + extenty.x, centery.y - extenty.y, centery.z + extenty.z);

        float min = 10000;
        for (int i = 0; i<8; i++)
        {
            float distance = Vector3.Distance(boundy.ClosestPoint(pointArrayX[i]), pointArrayX[i]);
            if (min > distance) min = distance;
        }

        for (int i = 0; i<8; i++)
        {
            float distance = Vector3.Distance(boundx.ClosestPoint(pointArrayY[i]), pointArrayY[i]);
            if (min > distance) min = distance;
        }

        return min;
    }


    Rect GetRectinCameraView(Bounds bound, float margin)
    {
        Vector3 centerx = bound.center;
        Vector3 extentx = bound.extents;

        Vector3[] pointArrayX = new Vector3[8];
        pointArrayX[0] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x - extentx.x, centerx.y + extentx.y, centerx.z - extentx.z));  // Front top left corner
        pointArrayX[1] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x + extentx.x, centerx.y + extentx.y, centerx.z - extentx.z));  // Front top right corner
        pointArrayX[2] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x - extentx.x, centerx.y - extentx.y, centerx.z - extentx.z));  // Front bottom left corner
        pointArrayX[3] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x + extentx.x, centerx.y - extentx.y, centerx.z - extentx.z));  // Front bottom right corner
        pointArrayX[4] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x - extentx.x, centerx.y + extentx.y, centerx.z + extentx.z));  // Back top left corner
        pointArrayX[5] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x + extentx.x, centerx.y + extentx.y, centerx.z + extentx.z));  // Back top right corner
        pointArrayX[6] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x - extentx.x, centerx.y - extentx.y, centerx.z + extentx.z));  // Back bottom left corner
        pointArrayX[7] = Camera.main.WorldToViewportPoint(new Vector3(centerx.x + extentx.x, centerx.y - extentx.y, centerx.z + extentx.z));  // Back bottom right corner

        Vector3 minX = new Vector3(1000.0f, 1000.0f, 1000.0f);
        Vector3 maxX = new Vector3(0, 0, 0);
        for (int i = 0; i < 8; i++)
        {
            if (pointArrayX[i].z > 0)
            {
                minX = Vector3.Min(minX, pointArrayX[i]);
                maxX = Vector3.Max(maxX, pointArrayX[i]);
            }
        }

        Rect rectX = Rect.MinMaxRect(minX.x - margin, minX.y - margin, maxX.x + margin, maxX.y + margin);
        return rectX;
    }

    bool WhetherInCameraView(Rect rect)
    {
        Rect rectScreen = Rect.MinMaxRect(0, 0, 1, 1);
        return rect.Overlaps(rectScreen);
    }

    bool occluded(Transform obj)
    {
        int layerMask = (1 << LayerMask.NameToLayer("wall"));

        Vector3 direction = Camera.main.transform.position - obj.position;
        if ( obj.name.Equals("instance_40"))
        {
            RaycastHit hit;
            if (Physics.Raycast(obj.position, direction, out hit,
                Vector3.Distance(Camera.main.transform.position, obj.position), layerMask))
            {
                Debug.DrawLine(obj.position, Camera.main.transform.position, Color.yellow);
                Debug.Log("hit: " + hit.transform.name);
                Debug.Log(Physics.Linecast(obj.position, Camera.main.transform.position, layerMask));
            }
        }

          return Physics.Linecast(obj.position, Camera.main.transform.position, layerMask);

    }

    bool occludedbound(Bounds bound)
    {
        Vector3 center = bound.center;
        int layerMask = (1 << LayerMask.NameToLayer("wall"));
        return Physics.Linecast(center, Camera.main.transform.position, layerMask);
    }

    void DrawBounds(Bounds bounds)
    {
        Vector3 v3Center = bounds.center;
        Vector3 v3Extents = bounds.extents;

        Vector3 v3FrontTopLeft;
        Vector3 v3FrontTopRight;
        Vector3 v3FrontBottomLeft;
        Vector3 v3FrontBottomRight;
        Vector3 v3BackTopLeft;
        Vector3 v3BackTopRight;
        Vector3 v3BackBottomLeft;
        Vector3 v3BackBottomRight;

        v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner
        v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner
        v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner
        v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner
        v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner
        v3BackTopRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner
        v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner
        v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner


        Color color = Color.green;

        Debug.DrawLine(v3FrontTopLeft, v3FrontTopRight, color);
        Debug.DrawLine(v3FrontTopRight, v3FrontBottomRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3FrontBottomLeft, color);
        Debug.DrawLine(v3FrontBottomLeft, v3FrontTopLeft, color);

        Debug.DrawLine(v3BackTopLeft, v3BackTopRight, color);
        Debug.DrawLine(v3BackTopRight, v3BackBottomRight, color);
        Debug.DrawLine(v3BackBottomRight, v3BackBottomLeft, color);
        Debug.DrawLine(v3BackBottomLeft, v3BackTopLeft, color);

        Debug.DrawLine(v3FrontTopLeft, v3BackTopLeft, color);
        Debug.DrawLine(v3FrontTopRight, v3BackTopRight, color);
        Debug.DrawLine(v3FrontBottomRight, v3BackBottomRight, color);
        Debug.DrawLine(v3FrontBottomLeft, v3BackBottomLeft, color);
    }
}
