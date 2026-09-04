using System.Collections.Generic;
using UnityEngine;

namespace DungeonDescent.Presentation
{
    public static class MaterialLibrary
    {
        private static readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();
        public static Material Stone => Get("Stone", "stone_albedo", new Color(.46f,.47f,.45f), .05f, .28f, "stone_normal");
        public static Material Wood => Get("Wood", "wood_albedo", new Color(.36f,.20f,.10f), .02f, .22f);
        public static Material Metal => Get("Iron", "metal_albedo", new Color(.18f,.20f,.22f), .72f, .38f);
        public static Material Cloth => Get("Cloth", "cloth_albedo", new Color(.20f,.07f,.08f), .0f, .18f);
        public static Material Moss => Get("Moss", "moss_albedo", new Color(.16f,.24f,.11f), .0f, .15f);
        public static Material Bone => GetColor("Bone", new Color(.66f,.63f,.52f), .0f, .22f);
        public static Material Skin => GetColor("Skin", new Color(.44f,.30f,.24f), .0f, .28f);
        public static Material Leather => GetColor("Leather", new Color(.13f,.075f,.045f), .0f, .2f);
        public static Material Black => GetColor("Blackened Steel", new Color(.045f,.05f,.06f), .72f, .3f);
        public static Material Water => GetColor("Flood Water", new Color(.035f,.17f,.19f), .14f, .68f, new Color(.01f,.09f,.11f));
        public static Material MagicBlue => GetColor("Ancient Azure", new Color(.04f,.15f,.22f), .18f, .45f, new Color(.02f,.35f,.65f)*2.2f);
        public static Material Fire => GetColor("Ember", new Color(.55f,.14f,.02f), .0f, .2f, new Color(1f,.16f,.01f)*4.5f);
        public static Material Blood => GetColor("Old Blood", new Color(.16f,.008f,.008f), .0f, .45f);

        private static Material Get(string name,string textureName,Color tint,float metallic,float smooth,string normal=null)
        {
            if(cache.TryGetValue(name,out var found)) return found;
            var m=Create(name,tint,metallic,smooth,Color.black);
            var tex=Resources.Load<Texture2D>("Textures/"+textureName); if(tex!=null)m.SetTexture("_BaseMap",tex);
            if(normal!=null){var n=Resources.Load<Texture2D>("Textures/"+normal);if(n!=null){m.SetTexture("_BumpMap",n);m.EnableKeyword("_NORMALMAP");}}
            cache[name]=m;return m;
        }
        private static Material GetColor(string name,Color c,float metallic,float smooth,Color emission=default)
        {if(cache.TryGetValue(name,out var found))return found;var m=Create(name,c,metallic,smooth,emission);cache[name]=m;return m;}
        private static Material Create(string name,Color c,float metallic,float smooth,Color emission)
        {
            var shader=Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var m=new Material(shader){name="DD "+name};
            if(m.HasProperty("_BaseColor"))m.SetColor("_BaseColor",c);else m.color=c;
            if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);
            if(emission.maxColorComponent>0f){m.EnableKeyword("_EMISSION");if(m.HasProperty("_EmissionColor"))m.SetColor("_EmissionColor",emission);}
            return m;
        }
    }
}
