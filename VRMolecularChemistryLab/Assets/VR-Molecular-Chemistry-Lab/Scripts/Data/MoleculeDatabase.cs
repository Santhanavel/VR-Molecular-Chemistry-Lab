using System.Collections.Generic;
using UnityEngine;

namespace VRMolecularLab.Data
{
    [CreateAssetMenu(fileName = "MoleculeDatabase", menuName = "VR Molecular Lab/Molecule Database")]
    public class MoleculeDatabase : ScriptableObject
    {
        public List<MoleculeData> molecules = new List<MoleculeData>();

        /// <summary>
        /// Attempts to find a molecule definition based on provided atom counts.
        /// </summary>
        public MoleculeData? TryGetMolecule(int h, int o, int c, int n)
        {
            foreach (var mol in molecules)
            {
                if (mol.Matches(h, o, c, n))
                {
                    return mol;
                }
            }
            return null;
        }

        /// <summary>
        /// Convenience method returning a boolean result while passing the out param.
        /// </summary>
        public bool TryFind(int h, int o, int c, int n, out MoleculeData result)
        {
            var found = TryGetMolecule(h, o, c, n);
            result = found ?? default;
            return found.HasValue;
        }
    }
}
