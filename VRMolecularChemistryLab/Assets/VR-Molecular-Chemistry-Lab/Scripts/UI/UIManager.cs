using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRMolecularLab.Core;
using VRMolecularLab.Data;

namespace VRMolecularLab.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Library Panel")]
        public Transform libraryGrid;
        public GameObject moleculeCardPrefab;
        private HashSet<string> _discovered = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed += OnMoleculeFormed;
                BondManager.Instance.OnMoleculeReset += OnMoleculeReset;
            }
        }

        private void OnDestroy()
        {
            if (BondManager.Instance != null)
            {
                BondManager.Instance.OnMoleculeFormed -= OnMoleculeFormed;
                BondManager.Instance.OnMoleculeReset -= OnMoleculeReset;
            }
        }
        // --- Library Logic ---
        public void OnMoleculeFormed(MoleculeData data)
        {
            AddDiscovery(data);
        }

        private void OnMoleculeReset(MoleculeData data)
        {
            // Optional: refresh library visuals if resetting modifies display state
        }

        public void AddDiscovery(MoleculeData mol)
        {
            if (_discovered.Contains(mol.moleculeName)) return;

            _discovered.Add(mol.moleculeName);

            if (moleculeCardPrefab != null && libraryGrid != null)
            {
                var cardObj = Instantiate(moleculeCardPrefab, libraryGrid);
                var card = cardObj.GetComponent<MoleculeCard>();
                if (card != null)
                {
                    card.Populate(mol);
                }

                // Replacing DOTween pop animation with Coroutine to prevent package missing errors
                StartCoroutine(ScalePunchAnim(cardObj.transform, 0.35f));
            }
        }

        public void ClearLibrary()
        {
            _discovered.Clear();
            foreach (Transform child in libraryGrid)
            {
                Destroy(child.gameObject);
            }
        }
        // --- Animation Coroutine Fallbacks (replaces DOTween dependency) ---
        private IEnumerator ScalePunchAnim(Transform target, float duration)
        {
            target.localScale = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Simple ease-out back approximation
                float curve = Mathf.Sin(t * Mathf.PI * 0.5f) + Mathf.Sin(t * Mathf.PI) * 0.2f;
                target.localScale = Vector3.one * Mathf.Clamp01(curve);
                yield return null;
            }
            target.localScale = Vector3.one;
        }
    }
}
