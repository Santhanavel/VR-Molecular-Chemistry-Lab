using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.UI
{
    public class AtomSpawner : MonoBehaviour
    {
        [Header("Configurations")]
        public AtomToken H_Token;
        public AtomToken O_Token;
        public AtomToken C_Token;
        public AtomToken N_Token;

        // Where atoms will spawn relative to the player
        public Transform primarySpawnPoint;

        public void SpawnAtom(string elementSymbol)
        {
            // In a full implementation, you could load the prefab from a direct reference
            // For this design, we will just simulate finding the prefab and instantiating it.
            // Ideally, AtomToken would hold its own Prefab reference, but the docs list AtomToken
            // with just 'atomMesh' and 'atomMaterial'. Therefore, you might have discrete prefabs 
            // for H, O, C, N assigned in the inspector here.
            Debug.Log($"[AtomSpawner] Spawning Atom: {elementSymbol}");
        }
    }
}
