using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aethernia.Editor.Environment
{
    public static class EnvironmentPhysicsSnapshot
    {
        public static string Capture(Scene scene)
        {
            Physics.SyncTransforms(); var lines = new List<string>();
            foreach (var root in scene.GetRootGameObjects()) foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                if (collider.GetComponentInParent<Aethernia.ExplorationUnit>() != null) continue;
                var path = collider.name; for(var p=collider.transform.parent;p!=null;p=p.parent) path=p.name+"/"+path;
                var b=collider.bounds;
                lines.Add(path+"|"+collider.GetType().Name+"|"+collider.enabled+"|"+collider.gameObject.activeInHierarchy+"|"+collider.isTrigger+"|"+V(b.center)+"|"+V(b.size));
            }
            lines.Sort(System.StringComparer.Ordinal); return string.Join("\n",lines);
        }
        private static string V(Vector3 v) => v.x.ToString("F4",CultureInfo.InvariantCulture)+","+v.y.ToString("F4",CultureInfo.InvariantCulture)+","+v.z.ToString("F4",CultureInfo.InvariantCulture);
    }
}
