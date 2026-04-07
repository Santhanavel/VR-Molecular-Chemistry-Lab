using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Core;
using VRMolecularLab.Data;

namespace VRMolecularLab.Editor
{
    public class MoleculePrefabGenerator
    {
        [MenuItem("VRLab/Auto-Generate All 18 Molecule Prefabs")]
        public static void GenerateAllMolecules()
        {
            string dbPath = "Assets/VR-Molecular-Chemistry-Lab/ScriptableObjects/MoleculeDatabase.asset";
            MoleculeDatabase db = AssetDatabase.LoadAssetAtPath<MoleculeDatabase>(dbPath);

            if (db == null)
            {
                Debug.LogError("Database not found! Please import the JSON first.");
                return;
            }

            string targetFolder = "Assets/VR-Molecular-Chemistry-Lab/Prefabs/Molecules";
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                System.IO.Directory.CreateDirectory(targetFolder);
            }

            // Load Materials to apply
            Material matH = AssetDatabase.LoadAssetAtPath<Material>("Assets/VR-Molecular-Chemistry-Lab/Materials/Atoms/Atom_H_Mat.mat");
            Material matO = AssetDatabase.LoadAssetAtPath<Material>("Assets/VR-Molecular-Chemistry-Lab/Materials/Atoms/Atom_O_Mat.mat");
            Material matC = AssetDatabase.LoadAssetAtPath<Material>("Assets/VR-Molecular-Chemistry-Lab/Materials/Atoms/Atom_C_Mat.mat");
            Material matN = AssetDatabase.LoadAssetAtPath<Material>("Assets/VR-Molecular-Chemistry-Lab/Materials/Atoms/Atom_N_Mat.mat");

            for (int i = 0; i < db.molecules.Count; i++)
            {
                MoleculeData data = db.molecules[i];
                string cleanName = data.moleculeName.Replace(" ", "");
                GameObject root = new GameObject($"Molecule_{cleanName}");

                // Components
                MoleculeInstance instance = root.AddComponent<MoleculeInstance>();
                Rigidbody rb = root.AddComponent<Rigidbody>();
                rb.mass = 0.1f;
                rb.useGravity = false;

                BoxCollider box = root.AddComponent<BoxCollider>();
                box.size = new Vector3(0.4f, 0.4f, 0.4f);

                root.AddComponent<XRGrabInteractable>();

                // Hierarchy
                GameObject atomsGroup = new GameObject("Atoms_Group");
                atomsGroup.transform.SetParent(root.transform);
                GameObject bondsGroup = new GameObject("Bonds_Group");
                bondsGroup.transform.SetParent(root.transform);
                GameObject labelRoot = new GameObject("Label_Root");
                labelRoot.transform.SetParent(root.transform);

                // Build Atoms List based on counts
                List<Material> atomMaterials = new List<Material>();
                for (int h = 0; h < data.hydrogenCount; h++) atomMaterials.Add(matH);
                for (int c = 0; c < data.carbonCount; c++) atomMaterials.Add(matC);
                for (int n = 0; n < data.nitrogenCount; n++) atomMaterials.Add(matN);
                for (int o = 0; o < data.oxygenCount; o++) atomMaterials.Add(matO);

                // Distribute Atoms Visually in a basic circle/line shape
                List<Vector3> positions = new List<Vector3>();
                for (int a = 0; a < atomMaterials.Count; a++)
                {
                    GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    atom.name = $"Atom_{a}";
                    atom.transform.SetParent(atomsGroup.transform);
                    atom.transform.localScale = Vector3.one * 0.1f;
                    
                    if (atomMaterials[a] != null)
                    {
                        atom.GetComponent<MeshRenderer>().sharedMaterial = atomMaterials[a];
                    }
                    
                    // Remove default colliders from visual primitives
                    GameObject.DestroyImmediate(atom.GetComponent<Collider>());

                    // Simple mathematical layout: radial placement
                    float radius = atomMaterials.Count == 1 ? 0f : 0.15f;
                    float angle = a * Mathf.PI * 2f / atomMaterials.Count;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    
                    // Specific override for bent water (H2O implies 3 atoms)
                    if (data.formula == "H2O")
                    {
                        if (a == 0) pos = new Vector3(-0.1f, -0.05f, 0); // H
                        if (a == 1) pos = new Vector3(0.1f, -0.05f, 0);  // H
                        if (a == 2) pos = new Vector3(0, 0.05f, 0);      // O
                    }

                    atom.transform.localPosition = pos;
                    positions.Add(pos);
                }

                // Add simple single-central-cylinder for bond visuals (placeholder connecting center)
                if (positions.Count > 1)
                {
                    for (int b = 0; b < positions.Count; b++)
                    {
                        GameObject bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        bond.name = $"Bond_{b}";
                        bond.transform.SetParent(bondsGroup.transform);
                        GameObject.DestroyImmediate(bond.GetComponent<Collider>());

                        // Point bond towards center
                        bond.transform.localPosition = positions[b] / 2f;
                        bond.transform.up = positions[b].normalized;
                        bond.transform.localScale = new Vector3(0.015f, positions[b].magnitude / 2f, 0.015f);
                    }
                }

                // Temporary generic text object label
                GameObject textObj = new GameObject("MoleculeName");
                textObj.transform.SetParent(labelRoot.transform);
                textObj.transform.localPosition = new Vector3(0, 0.25f, 0);
                // Note: Standard TextMesh is used here safely instead of TMP to avoid font asset dependency linking in editor script
                TextMesh tm = textObj.AddComponent<TextMesh>();
                tm.text = data.formula;
                tm.characterSize = 0.05f;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.color = Color.white;

                // Save Prefab
                string path = $"{targetFolder}/Molecule_{cleanName}.prefab";
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
                GameObject.DestroyImmediate(root);

                // Link to Database
                if (prefab != null)
                {
                    data.moleculePrefab = prefab;
                    db.molecules[i] = data;
                }
            }

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();

            Debug.Log("[Generator] All 18 Molecule Prefabs have been successfully built and connected to the database!");
        }
    }
}
