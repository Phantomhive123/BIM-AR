using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using UnityEngine;
using Xbim.Ifc2x3.PresentationResource;

public class Click : MonoBehaviour
{
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var raycastHit);
            if (raycastHit.collider != null)
            {
                Debug.Log(raycastHit.normal);//注意这个是点击到点的世界坐标下的法线
                Debug.Log(raycastHit.triangleIndex);//注意这个获得的是meshcollider的序列

                if (raycastHit.collider.GetComponent<MeshCollider>() == null) return;

                Mesh mesh = raycastHit.collider.GetComponent<MeshFilter>().sharedMesh;
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;

                List<int> finalVerticeIndex = new List<int>();//最终结果的顶点
                Vector3 aimNormal = normals[triangles[raycastHit.triangleIndex * 3]];//获取目标的相对法线

                //将点击到三角形的三个顶点加入最终结果
                finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3]);
                finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3 + 1]);
                finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3 + 2]);

                //获取法线符合要求的点
                List<int> parallelPoints = new List<int>();
                for (int i = 0; i < normals.Length; i++)
                {
                    if (normals[i] == aimNormal)
                        parallelPoints.Add(i);
                }

                //获取法线符合要求的三角面
                List<int> parallelTriangles = new List<int>();
                foreach(int i in triangles)
                {
                    if (parallelPoints.Contains(i))//之所以可以碰到就加入，是因为unity内置的mesh每个顶点都有三份，只要记录符合法线要求的即可，如果之后出现问题，大概率是在这里
                        parallelTriangles.Add(i);
                }

                //开始获取共面的顶点
                int finalVerticesCount = -1;
                int[] index = new int[3];
                do
                {
                    finalVerticesCount = finalVerticeIndex.Count;

                    for (int i = 0; i < parallelTriangles.Count;)
                    {
                        //逐三个读取顶点，获得三角形
                        index[0] = parallelTriangles[i++];
                        index[1] = parallelTriangles[i++];
                        index[2] = parallelTriangles[i++];

                        //一旦有一个在最终的面上，三个都在最终的面上
                        if (finalVerticeIndex.Contains(index[0]) || finalVerticeIndex.Contains(index[1]) || finalVerticeIndex.Contains(index[2]))
                        {
                            if (!finalVerticeIndex.Contains(index[0])) finalVerticeIndex.Add(index[0]);
                            if (!finalVerticeIndex.Contains(index[1])) finalVerticeIndex.Add(index[1]);
                            if (!finalVerticeIndex.Contains(index[2])) finalVerticeIndex.Add(index[2]);
                        }
                    }
                } while (finalVerticesCount == finalVerticeIndex.Count);//本次循环没有新增点，说明循环结束

                //遍历平行的三角面，获得所有共面的三角面
                List<int> finalTriangles = new List<int>();
                foreach (int i in parallelTriangles)
                    if (finalVerticeIndex.Contains(i))
                        finalTriangles.Add(i);

                //到这里为止还可以优化，将vertices和normal里面没有用到的都删掉

                Mesh newMesh = new Mesh();
                newMesh.vertices = vertices;
                newMesh.normals = normals;
                newMesh.triangles = finalTriangles.ToArray();
                raycastHit.collider.GetComponent<MeshFilter>().sharedMesh = newMesh;

                GameObject obj = new GameObject();
                MeshFilter mf = obj.AddComponent<MeshFilter>();
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();

                mr.material = new Material(Shader.Find("Standard"));
                mf.mesh = newMesh;
                obj.layer = LayerMask.NameToLayer("OutLine");
            }
        }
    }
}
