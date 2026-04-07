using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Core;

namespace VRMolecularLab.Editor
{
    public class MoleculeTemplateGenerator
    {
        [MenuItem("VRLab/Generate Molecule Template Prefab")]
        public static void GenerateTemplate()
        {
            string folderPath = "Assets/VR-Molecular-Chemistry-Lab/Prefabs/Molecules";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            // 1. Create the root object
            GameObject root = new GameObject("MoleculeTemplate");
            
            // 2. Add required root components based on DOC-05
            root.AddComponent<MoleculeInstance>();
            
            Rigidbody rb = root.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.useGravity = false;
            
            BoxCollider box = root.AddComponent<BoxCollider>();
            box.size = new Vector3(0.3f, 0.3f, 0.3f);
            
            root.AddComponent<XRGrabInteractable>();

            // 3. Create sub-groups for organization
            GameObject atomsGroup = new GameObject("Atoms_Group");
            atomsGroup.transform.SetParent(root.transform);
            
            GameObject bondsGroup = new GameObject("Bonds_Group");
            bondsGroup.transform.SetParent(root.transform);
            
            GameObject labelRoot = new GameObject("Label_Root");
            labelRoot.transform.SetParent(root.transform);
            
            // 4. Save and cleanup
            string prefabPath = $"{folderPath}/MoleculeTemplate.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            GameObject.DestroyImmediate(root);
            
            Debug.Log($"[TemplateGenerator] Created base molecule template at {prefabPath}");
        }
    }
}
