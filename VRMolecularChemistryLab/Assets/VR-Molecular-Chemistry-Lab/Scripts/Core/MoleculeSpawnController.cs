using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;
using VRMolecularLab.UI;

namespace VRMolecularLab.Core
{
    public class MoleculeSpawnController : MonoBehaviour
    {
        public static MoleculeSpawnController Instance { get; private set; }
        public Transform moleculeTargetAnchor;
        private MoleculeInstance _currentMolecule;
        private MoleculeData _currentData;

        private void Awake() { if (Instance != null && Instance != this) { Destroy(gameObject); return; } Instance = this; }

        public void SpawnOrUpgrade(MoleculeData newData, List<AtomController> placedAtoms)
        {
            if (_currentMolecule != null && _currentData.moleculeName == newData.moleculeName) return; 

            if (_currentMolecule != null)
            {
                Destroy(_currentMolecule.gameObject);
                _currentMolecule = null;
            }

            if (newData.moleculePrefab != null && moleculeTargetAnchor != null)
            {
                var obj = Instantiate(newData.moleculePrefab, moleculeTargetAnchor.position, Quaternion.identity);
                _currentMolecule = obj.GetComponent<MoleculeInstance>();
                if (_currentMolecule == null) _currentMolecule = obj.AddComponent<MoleculeInstance>();

                _currentMolecule.Data = newData;
                _currentMolecule.ConsumedAtoms = placedAtoms;
                _currentData = newData;

                if (AudioManager.Instance != null) AudioManager.Instance.PlayBondSuccess();
                if (UIManager.Instance != null) UIManager.Instance.AddDiscovery(newData);

                StartCoroutine(ScaleAnimation(obj.transform, 0.5f));
            }
        }

        public void ClearCurrentMolecule()
        {
            if (_currentMolecule != null && _currentMolecule.gameObject != null) Destroy(_currentMolecule.gameObject);
            _currentMolecule = null;
            _currentData = default;
        }

        private IEnumerator ScaleAnimation(Transform target, float duration)
        {
            target.localScale = Vector3.zero;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                if (target == null) yield break;
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsed / duration);
                yield return null;
            }
            if (target != null) target.localScale = Vector3.one;
        }
    }
}
