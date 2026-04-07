using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Editor
{
    // A wrapper class required for JsonUtility to properly parse a list/array
    [Serializable]
    public class MoleculeDataList
    {
        public List<MoleculeData> items;
    }

    public class MoleculeDatabaseImporter
    {
        [MenuItem("VRLab/Import Molecule Database")]
        public static void ImportDatabase()
        {
            // Set the paths based on actual structured approach
            string dbPath = "Assets/VR-Molecular-Chemistry-Lab/ScriptableObjects/MoleculeDatabase.asset";
            string jsonPath = "Assets/VR-Molecular-Chemistry-Lab/Data/molecules.json";

            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"[Importer] JSON not found at: {jsonPath}");
                return;
            }

            var db = AssetDatabase.LoadAssetAtPath<MoleculeDatabase>(dbPath);

            // If the database asset doesn't exist, create it automatically
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<MoleculeDatabase>();
                
                // Ensure the folder exists
                if (!AssetDatabase.IsValidFolder("Assets/VR-Molecular-Chemistry-Lab/ScriptableObjects"))
                {
                    AssetDatabase.CreateFolder("Assets/VR-Molecular-Chemistry-Lab", "ScriptableObjects");
                }

                AssetDatabase.CreateAsset(db, dbPath);
                Debug.Log($"[Importer] Created new MoleculeDatabase at {dbPath}");
            }

            var json = File.ReadAllText(jsonPath);
            var data = JsonUtility.FromJson<MoleculeDataList>(json);

            if (data != null && data.items != null)
            {
                // Assign new items list
                db.molecules = data.items;
                
                EditorUtility.SetDirty(db);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                Debug.Log($"[Importer] Successfully imported {db.molecules.Count} molecules directly into {db.name}!");
            }
            else
            {
                Debug.LogError("[Importer] Failed to parse JSON or JSON items array was null.");
            }
        }
    }
}
