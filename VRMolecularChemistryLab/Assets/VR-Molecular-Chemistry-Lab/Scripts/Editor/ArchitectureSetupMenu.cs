using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Core;
using VRMolecularLab.Data;
using VRMolecularLab.UI;

namespace VRMolecularLab.Editor
{
    public class ArchitectureSetupMenu : EditorWindow
    {
        [MenuItem("VR Molecular Lab/Setup/Build New Architecture Hierarchy")]
        public static void BuildArchitecture()
        {
            // 1. Create Scene Managers
            CreateManager("[GameStateManager]", typeof(GameStateManager));
            CreateManager("[AtomSpawner]", typeof(AtomSpawner));
            CreateManager("[BondSocketManager]", typeof(BondSocketManager));
            CreateManager("[MoleculeSpawnController]", typeof(MoleculeSpawnController));
            
            // AudioManager needs 2 AudioSources
            GameObject audioObj = CreateManager("[AudioManager]", typeof(AudioManager));
            if (audioObj.GetComponents<AudioSource>().Length < 2)
            {
                audioObj.AddComponent<AudioSource>();
            }

            CreateManager("[BondManager]", typeof(BondManager));
            CreateManager("[UIManager]", typeof(UIManager));

            // 2. Create MoleculeTarget Anchor
            GameObject moleculeTarget = GameObject.Find("MoleculeTarget Anchor");
            if (moleculeTarget == null)
            {
                moleculeTarget = new GameObject("MoleculeTarget Anchor");
                // 0.6m in front of chest
                moleculeTarget.transform.position = new Vector3(0f, 1.2f, 0.6f); 
            }

            // 3. Create BondSocket Prefab if missing
            string prefabFolder = "Assets/VR-Molecular-Chemistry-Lab/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabFolder))
            {
                AssetDatabase.CreateFolder("Assets/VR-Molecular-Chemistry-Lab", "Prefabs");
            }

            string socketPrefabPath = $"{prefabFolder}/BondSocket.prefab";
            GameObject socketPrefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(socketPrefabPath);
            if (socketPrefabObj == null)
            {
                GameObject root = new GameObject("BondSocket");
                root.AddComponent<BondSocket>();
                SphereCollider rootCol = root.AddComponent<SphereCollider>();
                rootCol.isTrigger = true;
                rootCol.radius = 0.08f;

                GameObject vis = new GameObject("SocketVisual");
                vis.transform.SetParent(root.transform);
                vis.AddComponent<MeshFilter>(); // user can assign mesh later
                vis.AddComponent<MeshRenderer>();

                PrefabUtility.SaveAsPrefabAsset(root, socketPrefabPath);
                DestroyImmediate(root);
                Debug.Log($"Created BondSocket prefab at {socketPrefabPath}");
            }

            // 4. Auto-assigning references in the Scene
            GameStateManager gsm = FindObjectOfType<GameStateManager>();
            AtomSpawner atomSpawner = FindObjectOfType<AtomSpawner>();
            BondSocketManager bsm = FindObjectOfType<BondSocketManager>();
            MoleculeSpawnController msc = FindObjectOfType<MoleculeSpawnController>();
            UIManager uiManager = FindObjectOfType<UIManager>();

            if (gsm != null)
            {
                gsm.atomSpawner = atomSpawner;
                gsm.bondSocketManager = bsm;
                gsm.moleculeSpawnController = msc;
                gsm.uiManager = uiManager;
            }

            if (msc != null)
            {
                msc.moleculeTargetAnchor = moleculeTarget.transform;
            }

            if (bsm != null && bsm.bondSocketPrefab == null)
            {
                bsm.bondSocketPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(socketPrefabPath);
                
                GameObject socketSpawnRoot = GameObject.Find("SocketSpawnRoot");
                if (socketSpawnRoot == null)
                {
                    socketSpawnRoot = new GameObject("SocketSpawnRoot");
                    socketSpawnRoot.transform.position = new Vector3(0.5f, 1.0f, 0.5f);
                }
                bsm.socketSpawnRoot = socketSpawnRoot.transform;
            }

            if (atomSpawner != null && (atomSpawner.spawnPoints == null || atomSpawner.spawnPoints.Length == 0))
            {
                GameObject spawnPointsRoot = GameObject.Find("AtomSpawnPoints");
                if (spawnPointsRoot == null) spawnPointsRoot = new GameObject("AtomSpawnPoints");
                
                atomSpawner.spawnPoints = new Transform[4];
                for (int i = 0; i < 4; i++)
                {
                    GameObject sp = new GameObject($"SpawnPoint_{i}");
                    sp.transform.SetParent(spawnPointsRoot.transform);
                    sp.transform.position = new Vector3(-0.3f + (0.2f * i), 1.1f, 0.4f);
                    atomSpawner.spawnPoints[i] = sp.transform;
                }
            }

            Debug.Log("Scene hierarchy setup complete! Please manually assemble Atom prefabs and assign UI Buttons.");
        }

        private static GameObject CreateManager(string name, System.Type scriptType)
        {
            GameObject obj = GameObject.Find(name);
            if (obj == null)
            {
                obj = new GameObject(name);
            }
            if (obj.GetComponent(scriptType) == null)
            {
                obj.AddComponent(scriptType);
            }
            return obj;
        }

        [MenuItem("VR Molecular Lab/Setup/Build Base Atom Prefab Template")]
        public static void BuildAtomTemplate()
        {
            string prefabFolder = "Assets/VR-Molecular-Chemistry-Lab/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabFolder))
            {
                AssetDatabase.CreateFolder("Assets/VR-Molecular-Chemistry-Lab", "Prefabs");
            }

            string path = $"{prefabFolder}/AtomTemplate.prefab";

            GameObject root = new GameObject("AtomTemplate");
            
            // Add components
            root.AddComponent<XRGrabInteractable>();
            root.AddComponent<AtomController>();
            root.AddComponent<AntigravityFloat>();
            
            Rigidbody rb = root.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            root.AddComponent<MeshFilter>();
            root.AddComponent<MeshRenderer>();
            
            SphereCollider sc = root.AddComponent<SphereCollider>();
            sc.isTrigger = false;

            GameObject child = new GameObject("SocketDetector");
            child.transform.SetParent(root.transform);
            SphereCollider childSc = child.AddComponent<SphereCollider>();
            childSc.isTrigger = true;
            childSc.radius = 0.08f;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            DestroyImmediate(root);

            Debug.Log($"Created Atom Template Base Prefab at {path}. Duplicate this for H, O, C, N and assign it in their Tokens.");
        }
    }
}
