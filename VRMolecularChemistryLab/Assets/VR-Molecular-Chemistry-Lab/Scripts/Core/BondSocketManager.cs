using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.UI;
using VRMolecularLab.Data;

namespace VRMolecularLab.Core
{
    public class BondSocketManager : MonoBehaviour
    {
        public static BondSocketManager Instance { get; private set; }

        public GameObject bondSocketPrefab;
        public Transform socketSpawnRoot;
        public float socketOffset = 0.15f;
        private List<BondSocket> _activeSockets = new List<BondSocket>();
        private List<AtomController> _placedAtoms = new List<AtomController>();
        private const int MAX_SOCKETS = 8;
        
        private int _grabCount = 0;
        public bool IsSockedVisible => _grabCount > 0;

        private void Awake() { if (Instance != null && Instance != this) { Destroy(gameObject); return; } Instance = this; }

        public void OpenFirstSocket()
        {
            if (bondSocketPrefab == null || socketSpawnRoot == null || _activeSockets.Count >= MAX_SOCKETS) return;
            var obj = Instantiate(bondSocketPrefab, socketSpawnRoot.position, Quaternion.identity, socketSpawnRoot);
            var socket = obj.GetComponent<BondSocket>();
            if (socket != null)
            {
                _activeSockets.Add(socket);
                if (socket.socketVisual != null) socket.socketVisual.SetActive(IsSockedVisible);
            }
        }

        public void OnSocketOccupied(BondSocket filledSocket, AtomController atom)
        {
            if (atom == null) return;
            _placedAtoms.Add(atom);

            if (AtomSpawner.Instance != null && atom.atomData != null)
                AtomSpawner.Instance.RespawnAtom(atom.atomData);

            if (_activeSockets.Count < MAX_SOCKETS)
            {
                float xOff = socketOffset * _activeSockets.Count;
                float yOff = (_activeSockets.Count % 3 == 0) ? socketOffset * 0.5f : 0f;
                var nextPos = socketSpawnRoot.position + new Vector3(xOff, yOff, 0);
                var nextObj = Instantiate(bondSocketPrefab, nextPos, Quaternion.identity, socketSpawnRoot);
                var nextSocket = nextObj.GetComponent<BondSocket>();
                if (nextSocket != null)
                {
                    _activeSockets.Add(nextSocket);
                    if (nextSocket.socketVisual != null) nextSocket.socketVisual.SetActive(IsSockedVisible);
                }
            }

            if (BondManager.Instance != null)
            {
                var eval = BondManager.Instance.EvaluatePlacedAtoms(_placedAtoms);
                if (eval.isImpossible) filledSocket.FlashInvalid();
                else if (eval.result.HasValue && MoleculeSpawnController.Instance != null)
                    MoleculeSpawnController.Instance.SpawnOrUpgrade(eval.result.Value, new List<AtomController>(_placedAtoms));
            }
        }

        public void ResetAllSockets()
        {
            foreach (var s in _activeSockets) if (s != null && s.gameObject != null) Destroy(s.gameObject);
            _activeSockets.Clear();
            _placedAtoms.Clear();
            _grabCount = 0; // fallback reset
        }

        public void RegisterGrab()
        {
            _grabCount++;
            UpdateVisibility();
        }

        public void UnregisterGrab()
        {
            _grabCount--;
            if (_grabCount < 0) _grabCount = 0;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            foreach (var socket in _activeSockets)
            {
                if (socket != null && !socket.IsOccupied && socket.socketVisual != null)
                {
                    socket.socketVisual.SetActive(IsSockedVisible);
                }
            }
        }
    }
}
