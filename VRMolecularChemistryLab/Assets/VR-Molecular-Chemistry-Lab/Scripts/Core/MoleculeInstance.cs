using System.Collections.Generic;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Core
{
    public class MoleculeInstance : MonoBehaviour
    {
        public MoleculeData Data;
        public List<AtomController> ConsumedAtoms = new List<AtomController>();

        /// <summary>
        /// Called by BondManager (or Inspector UI) to break the molecule apart.
        /// </summary>
        public void Dissolve()
        {
            if (ConsumedAtoms != null)
            {
                foreach (var atom in ConsumedAtoms)
                {
                    if (atom != null)
                    {
                        atom.ResetAtom();
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
