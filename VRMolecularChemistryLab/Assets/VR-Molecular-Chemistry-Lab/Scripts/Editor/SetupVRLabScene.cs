using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using VRMolecularLab.Core;
using VRMolecularLab.Data;
using VRMolecularLab.UI;

namespace VRMolecularLab.Editor
{
    public class SetupVRLabScene : EditorWindow
    {
        [MenuItem("VRLab/Setup VR Lab Scene Hierarchy")]
        public static void SetupScene()
        {
            // Ensure we are operating on the active scene
            Scene activeScene = EditorSceneManager.GetActiveScene();
            
            // 1. Create Managers
            GameObject managersObj = GameObject.Find("Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("Managers");
                // Ensure the tag exists before applying (optional, skipping exact tag apply to avoid errors if tag isn't declared yet)
            }

            // Attach core manager scripts if missing
            if (managersObj.GetComponent<BondManager>() == null)
                managersObj.AddComponent<BondManager>();

            if (managersObj.GetComponent<UIManager>() == null)
                managersObj.AddComponent<UIManager>();

            if (managersObj.GetComponent<AudioManager>() == null)
                managersObj.AddComponent<AudioManager>();

            // Assign Database Reference to BondManager if it exists
            BondManager bondManager = managersObj.GetComponent<BondManager>();
            if (bondManager.database == null)
            {
                string dbPath = "Assets/VR-Molecular-Chemistry-Lab/ScriptableObjects/MoleculeDatabase.asset";
                MoleculeDatabase db = AssetDatabase.LoadAssetAtPath<MoleculeDatabase>(dbPath);
                if (db != null)
                {
                    bondManager.database = db;
                }
            }

            // 2. Create UI_WorldSpace
            GameObject uiObj = GameObject.Find("UI_WorldSpace");
            if (uiObj == null)
            {
                uiObj = new GameObject("UI_WorldSpace");
                
                GameObject libraryPanel = new GameObject("LibraryPanel");
                libraryPanel.transform.SetParent(uiObj.transform);
                libraryPanel.transform.position = new Vector3(1.2f, 1.4f, 1.5f); // from doc
                libraryPanel.transform.rotation = Quaternion.Euler(0, -15f, 0);
                
                GameObject inspectorPanel = new GameObject("InspectorPanel");
                inspectorPanel.transform.SetParent(uiObj.transform);
                inspectorPanel.SetActive(false); // Default hidden
            }

            // 3. Create AtomSpawnPoints
            GameObject spawnPointsObj = GameObject.Find("AtomSpawnPoints");
            if (spawnPointsObj == null)
            {
                spawnPointsObj = new GameObject("AtomSpawnPoints");
                for (int i = 0; i < 4; i++)
                {
                    GameObject p = new GameObject($"SpawnPoint_{i}");
                    p.transform.SetParent(spawnPointsObj.transform);
                }
            }

            // Mark scene as dirty so the user can save the changes
            EditorUtility.SetDirty(managersObj);
            EditorSceneManager.MarkSceneDirty(activeScene);

            Debug.Log("[Setup] VR Lab Scene hierarchy and core scripts populated successfully!");
        }
    }
}
