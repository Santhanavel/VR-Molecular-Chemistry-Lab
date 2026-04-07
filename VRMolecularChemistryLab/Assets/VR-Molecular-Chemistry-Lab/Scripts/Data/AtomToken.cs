using UnityEngine;

namespace VRMolecularLab.Data
{
    [CreateAssetMenu(fileName = "NewAtomToken", menuName = "VR Molecular Lab/Atom Token")]
    public class AtomToken : ScriptableObject
    {
        [Tooltip("Symbol of the element e.g. H, O, C, N")]
        public string elementSymbol;
        
        [Tooltip("Full name of the element e.g. Hydrogen, Oxygen, Carbon, Nitrogen")]
        public string elementName;
        
        [Tooltip("CPK standard color for this element")]
        public Color atomColor = Color.white;
        
        [Tooltip("Material to apply to the atom mesh")]
        public Material atomMaterial;
        
        [Tooltip("Base mesh used to represent this atom")]
        public Mesh atomMesh;
        
        [Tooltip("Text label rendered on the atom")]
        public string labelText;
    }
}
