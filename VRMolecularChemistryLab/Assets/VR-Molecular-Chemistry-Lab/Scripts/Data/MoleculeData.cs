using System;
using UnityEngine;

namespace VRMolecularLab.Data
{
    public enum BondType
    {
        Single,
        Double,
        Triple
    }

    [Serializable]
    public struct MoleculeData
    {
        public string moleculeName;
        public string formula;
        
        [Header("Atom Configuration")]
        [Range(0, 5)] public int hydrogenCount;
        [Range(0, 2)] public int oxygenCount;
        [Range(0, 2)] public int carbonCount;
        [Range(0, 2)] public int nitrogenCount;
        
        public BondType bondType;
        public GameObject moleculePrefab;
        public Sprite discoveryIcon;
        
        [TextArea(2, 4)]
        public string description;

        /// <summary>
        /// Checks if this molecule matches the given atom counts.
        /// </summary>
        public bool Matches(int h, int o, int c, int n)
        {
            return hydrogenCount == h
                && oxygenCount == o
                && carbonCount == c
                && nitrogenCount == n;
        }
    }
}
