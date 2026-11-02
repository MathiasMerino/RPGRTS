using System;
using UnityEngine;
using Object=UnityEngine.Object;
namespace Aethernia
{
    // La representación nunca crea controladores, modifica estadísticas o carga partidas.
    public static class CharacterVisualFactory
    {
        public static bool TryCreate(CharacterVisualCatalog catalog,string id,Transform parent,out CharacterVisualModel model,out GameObject source,out string failure)
        {
            model=null;source=null;failure=null;var entry=catalog==null?null:catalog.FindEntry(id);if(entry==null)return false;
            GameObject previous=null;
            for(var i=0;i<4;i++)
            {
                var candidate=catalog.Candidate(entry,i,out var animated);if(candidate==null||candidate==previous)continue;previous=candidate;
                if(!CharacterVisualContract.CanUse(candidate,animated,out var reason,true)){failure=reason;continue;}
                GameObject node=null;
                try
                {
                    node=Object.Instantiate(candidate,parent,false);
                    if(!CharacterVisualContract.CanUse(node,animated,out reason,true)){failure=reason;Discard(node);continue;}
                    model=node.GetComponent<CharacterVisualModel>();node.SetActive(true);source=candidate;return true;
                }
                catch(Exception e){failure=e.GetType().Name;if(node!=null)Discard(node);}
            }
            return false;
        }
        private static void Discard(GameObject node){node.SetActive(false);if(Application.isPlaying)Object.Destroy(node);else Object.DestroyImmediate(node);}
    }
}
