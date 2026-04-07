using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Data;
using VRMolecularLab.Core;

namespace VRMolecularLab.UI
{
    public class AtomSpawner : MonoBehaviour
    {
        public static AtomSpawner Instance { get; private set; }
        public List<AtomToken> atomTokens;
        public Transform[] spawnPoints;

        private Dictionary<AtomController, Vector3> _homeMap = new Dictionary<AtomController, Vector3>();
        private List<GameObject> _spawnedAtoms = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void StartSession()
        {
            if (atomTokens == null || atomTokens.Count == 0 || spawnPoints == null || spawnPoints.Length == 0) return;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                AtomToken token = atomTokens[i % atomTokens.Count];
                if (token.atomPrefab != null)
                {
                    var obj = Instantiate(token.atomPrefab, spawnPoints[i].position, Quaternion.identity);
                    var ac = obj.GetComponent<AtomController>();
                    var af = obj.GetComponent<AntigravityFloat>();
                    if (af != null) af.SetHome(spawnPoints[i].position);
                    if (ac != null) _homeMap[ac] = spawnPoints[i].position;
                    _spawnedAtoms.Add(obj);
                }
            }
        }

        public Vector3 GetHomePosition(AtomController atom)
        {
            if (atom != null && _homeMap.TryGetValue(atom, out Vector3 hp)) return hp;
            return Vector3.zero;
        }

        public void RespawnAtom(AtomToken token)
        {
            Vector3 homeTarget = Vector3.zero;
            bool foundHome = false;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (atomTokens[i % atomTokens.Count] == token)
                {
                    homeTarget = spawnPoints[i].position;
                    foundHome = true;
                    break;
                }
            }
            if (!foundHome) return;

            foreach (var kvp in _homeMap)
                if (kvp.Key != null && kvp.Key.atomData == token && !kvp.Key.IsConsumed && !kvp.Key.IsPlaced && kvp.Value == homeTarget)
                    return;

            if (token.atomPrefab != null)
            {
                var obj = Instantiate(token.atomPrefab, homeTarget, Quaternion.identity);
                var ac = obj.GetComponent<AtomController>();
                var af = obj.GetComponent<AntigravityFloat>();
                if (af != null) af.SetHome(homeTarget);
                if (ac != null) _homeMap[ac] = homeTarget;
                _spawnedAtoms.Add(obj);
            }
        }

        public void ResetAll()
        {
            foreach (var obj in _spawnedAtoms)
            {
                if (obj != null)
                {
                    var xri = obj.GetComponent<XRGrabInteractable>();
                    if (xri != null) xri.enabled = false;
                    Destroy(obj);
                }
            }
            _spawnedAtoms.Clear();
            _homeMap.Clear();
        }

        public void CleanDesk()
        {
            var newMap = new Dictionary<AtomController, Vector3>();
            for (int i = _spawnedAtoms.Count - 1; i >= 0; i--)
            {
                var obj = _spawnedAtoms[i];
                if (obj == null) continue;

                var ac = obj.GetComponent<AtomController>();
                if (ac != null && !ac.IsPlaced && !ac.IsConsumed)
                {
                    if (_homeMap.TryGetValue(ac, out Vector3 home))
                    {
                        newMap[ac] = home;
                    }
                }
                else if (ac != null)
                {
                    Destroy(obj);
                    _spawnedAtoms.RemoveAt(i);
                }
            }
            
            _homeMap = newMap;
            _spawnedAtoms.RemoveAll(x => x == null);

            if (atomTokens != null && spawnPoints != null)
            {
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    AtomToken token = atomTokens[i % atomTokens.Count];
                    RespawnAtom(token);
                }
            }
        }
    }
}
