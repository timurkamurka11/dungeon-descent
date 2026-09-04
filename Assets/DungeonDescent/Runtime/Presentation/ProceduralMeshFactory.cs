using System.Collections.Generic;
using UnityEngine;

namespace DungeonDescent.Presentation
{
    public static class ProceduralMeshFactory
    {
        private static Mesh box, cylinder, sphere, cone;
        public static Mesh Box => box != null ? box : box = BuildBox();
        public static Mesh Cylinder => cylinder != null ? cylinder : cylinder = BuildCylinder(16);
        public static Mesh Sphere => sphere != null ? sphere : sphere = BuildSphere(16, 10);
        public static Mesh Cone => cone != null ? cone : cone = BuildCone(16);

        private static Mesh BuildBox()
        {
            var m = new Mesh { name = "DD Architectural Block" };
            var v = new[] {
                new Vector3(-.5f,-.5f,.5f),new Vector3(.5f,-.5f,.5f),new Vector3(.5f,.5f,.5f),new Vector3(-.5f,.5f,.5f),
                new Vector3(.5f,-.5f,-.5f),new Vector3(-.5f,-.5f,-.5f),new Vector3(-.5f,.5f,-.5f),new Vector3(.5f,.5f,-.5f),
                new Vector3(-.5f,-.5f,-.5f),new Vector3(-.5f,-.5f,.5f),new Vector3(-.5f,.5f,.5f),new Vector3(-.5f,.5f,-.5f),
                new Vector3(.5f,-.5f,.5f),new Vector3(.5f,-.5f,-.5f),new Vector3(.5f,.5f,-.5f),new Vector3(.5f,.5f,.5f),
                new Vector3(-.5f,.5f,.5f),new Vector3(.5f,.5f,.5f),new Vector3(.5f,.5f,-.5f),new Vector3(-.5f,.5f,-.5f),
                new Vector3(-.5f,-.5f,-.5f),new Vector3(.5f,-.5f,-.5f),new Vector3(.5f,-.5f,.5f),new Vector3(-.5f,-.5f,.5f)
            };
            var t = new int[36]; for (var f=0;f<6;f++){var o=f*4;var q=f*6;t[q]=o;t[q+1]=o+1;t[q+2]=o+2;t[q+3]=o;t[q+4]=o+2;t[q+5]=o+3;}
            var uv = new Vector2[24]; for(var f=0;f<6;f++){var o=f*4;uv[o]=Vector2.zero;uv[o+1]=Vector2.right;uv[o+2]=Vector2.one;uv[o+3]=Vector2.up;}
            m.vertices=v; m.triangles=t; m.uv=uv; m.RecalculateNormals(); m.RecalculateTangents(); m.RecalculateBounds(); return m;
        }

        private static Mesh BuildCylinder(int sides)
        {
            var verts=new List<Vector3>(); var uv=new List<Vector2>(); var tris=new List<int>();
            for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;float x=Mathf.Cos(a)*.5f,z=Mathf.Sin(a)*.5f;verts.Add(new Vector3(x,-.5f,z));verts.Add(new Vector3(x,.5f,z));uv.Add(new Vector2((float)i/sides,0));uv.Add(new Vector2((float)i/sides,1));}
            for(int i=0;i<sides;i++){int o=i*2;tris.Add(o);tris.Add(o+1);tris.Add(o+3);tris.Add(o);tris.Add(o+3);tris.Add(o+2);}
            int bottom=verts.Count; verts.Add(new Vector3(0,-.5f,0)); uv.Add(new Vector2(.5f,.5f));
            int top=verts.Count; verts.Add(new Vector3(0,.5f,0)); uv.Add(new Vector2(.5f,.5f));
            for(int i=0;i<sides;i++){int a=i*2,b=(i+1)*2;tris.Add(bottom);tris.Add(b);tris.Add(a);tris.Add(top);tris.Add(a+1);tris.Add(b+1);}
            var m=new Mesh{name="DD Column"};m.SetVertices(verts);m.SetUVs(0,uv);m.SetTriangles(tris,0);m.RecalculateNormals();m.RecalculateTangents();m.RecalculateBounds();return m;
        }

        private static Mesh BuildSphere(int lon, int lat)
        {
            var verts=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();
            for(int y=0;y<=lat;y++){float v=(float)y/lat;float phi=v*Mathf.PI;for(int x=0;x<=lon;x++){float u=(float)x/lon;float th=u*Mathf.PI*2;verts.Add(new Vector3(Mathf.Sin(phi)*Mathf.Cos(th),Mathf.Cos(phi),Mathf.Sin(phi)*Mathf.Sin(th))*.5f);uv.Add(new Vector2(u,v));}}
            for(int y=0;y<lat;y++)for(int x=0;x<lon;x++){int a=y*(lon+1)+x,b=a+lon+1;tris.Add(a);tris.Add(b);tris.Add(a+1);tris.Add(a+1);tris.Add(b);tris.Add(b+1);}
            var m=new Mesh{name="DD Rounded Form"};m.SetVertices(verts);m.SetUVs(0,uv);m.SetTriangles(tris,0);m.RecalculateNormals();m.RecalculateTangents();m.RecalculateBounds();return m;
        }

        private static Mesh BuildCone(int sides)
        {
            var verts=new List<Vector3>{new Vector3(0,.5f,0),new Vector3(0,-.5f,0)};var uv=new List<Vector2>{new Vector2(.5f,1),new Vector2(.5f,.5f)};var tris=new List<int>();
            for(int i=0;i<sides;i++){float a=i*Mathf.PI*2/sides;verts.Add(new Vector3(Mathf.Cos(a)*.5f,-.5f,Mathf.Sin(a)*.5f));uv.Add(new Vector2((float)i/sides,0));}
            for(int i=0;i<sides;i++){int a=2+i,b=2+(i+1)%sides;tris.Add(0);tris.Add(a);tris.Add(b);tris.Add(1);tris.Add(b);tris.Add(a);}
            var m=new Mesh{name="DD Tapered Form"};m.SetVertices(verts);m.SetUVs(0,uv);m.SetTriangles(tris,0);m.RecalculateNormals();m.RecalculateTangents();m.RecalculateBounds();return m;
        }
    }
}
