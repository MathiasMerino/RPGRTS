using UnityEngine;

namespace Aethernia
{
    public readonly struct CombatFeedback
    {
        public readonly int EncounterId;
        public readonly CombatResolution Result;
        public readonly string ClassId;
        public readonly Vector3 Source, Target;
        public CombatFeedback(int encounter,CombatResolution result,string id,Vector3 source,Vector3 target)
        {EncounterId=encounter;Result=result;ClassId=id;Source=source;Target=target;}
    }
}
