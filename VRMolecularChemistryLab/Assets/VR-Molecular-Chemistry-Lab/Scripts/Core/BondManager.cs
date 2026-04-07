using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Core
{
    public class BondManager : MonoBehaviour
    {
        public static BondManager Instance { get; private set; }

        [Header("Configuration")]
        public MoleculeDatabase database;
        public Transform moleculeSpawnPoint;
        public float bondCooldown = 0.5f;

        // Events
        public event Action<MoleculeData> OnMoleculeFormed;
        public event Action OnBondFailed;
        public event Action<MoleculeData> OnMoleculeReset;

        private float _nextBondTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OnAtomProximity(AtomController trigger, List<AtomController> nearby)
        {
            if (Time.time < _nextBondTime) return;

            // Collect all candidates: trigger + nearby (exclude consumed)
            var candidates = new List<AtomController> { trigger };
            candidates.AddRange(nearby.Where(a => !a.IsConsumed));

            // Count by element
            int h = candidates.Count(a => a.ElementSymbol == "H");
            int o = candidates.Count(a => a.ElementSymbol == "O");
            int c = candidates.Count(a => a.ElementSymbol == "C");
            int n = candidates.Count(a => a.ElementSymbol == "N");

            if (database != null && database.TryFind(h, o, c, n, out var molData))
            {
                FormMolecule(molData, candidates);
            }
            else
            {
                // Optional: Provide fail feedback (shake, buzzer)
                // Note: since this happens rapidly in FixedUpdate, we rely on the cooldown
                // to prevent continuous buzzing
                OnBondFailed?.Invoke();
            }

            _nextBondTime = Time.time + bondCooldown;
        }

        private void FormMolecule(MoleculeData data, List<AtomController> atoms)
        {
            // Calculate a center point among the atoms if a fixed spawn point isn't set
            Vector3 spawnPos = moleculeSpawnPoint != null ? moleculeSpawnPoint.position : GetCenterOfAtoms(atoms);

            if (data.moleculePrefab != null)
            {
                var molObj = Instantiate(data.moleculePrefab, spawnPos, Quaternion.identity);

                var molInstance = molObj.GetComponent<MoleculeInstance>();
                if (molInstance == null)
                {
                    molInstance = molObj.AddComponent<MoleculeInstance>();
                }
                
                molInstance.Data = data;
                molInstance.ConsumedAtoms = new List<AtomController>(atoms);

                foreach (var atom in atoms)
                {
                    atom.ConsumeIntoMolecule(molObj);
                }

                OnMoleculeFormed?.Invoke(data);
            }
            else
            {
                Debug.LogWarning($"[BondManager] Missing Molecule Prefab for {data.moleculeName}");
            }
        }

        /// <summary>
        /// Resets a formed molecule, restoring its constituent atoms to their original states.
        /// </summary>
        public void ResetMolecule(GameObject molObj)
        {
            var molInstance = molObj.GetComponent<MoleculeInstance>();
            if (molInstance != null)
            {
                var data = molInstance.Data;
                molInstance.Dissolve(); // Destroy and cleanup is handled inside Dissolve
                
                OnMoleculeReset?.Invoke(data);
            }
        }

        private Vector3 GetCenterOfAtoms(List<AtomController> atoms)
        {
            if (atoms.Count == 0) return Vector3.zero;
            Vector3 sum = Vector3.zero;
            foreach (var a in atoms) sum += a.transform.position;
            return sum / atoms.Count;
        }
    }
}
