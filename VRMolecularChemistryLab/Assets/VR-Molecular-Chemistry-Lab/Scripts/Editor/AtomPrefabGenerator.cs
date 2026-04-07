using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRMolecularLab.Core;
using VRMolecularLab.Data;
using TMPro;

namespace VRMolecularLab.Editor
{
    public class AtomPrefabGenerator
    {
        [MenuItem("VRLab/Generate 4 Grabbable Atom Prefabs")]
        public static void GenerateAtomPrefabs()
        {
            string targetFolder = "Assets/VR-Molecular-Chemistry-Lab/Prefabs/Atoms";
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                System.IO.Directory.CreateDirectory(targetFolder);
                AssetDatabase.Refresh();
            }

            // Create Atom Layer if it doesn't exist
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            bool layerExists = false;
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == "Atom")
                {
                    layerExists = true;
                    break;
                }
            }

            if (!layerExists)
            {
                for (int i = 8; i < layers.arraySize; i++)
                {
                    SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                    if (string.IsNullOrEmpty(layerSP.stringValue))
                    {
                        layerSP.stringValue = "Atom";
                        tagManager.ApplyModifiedProperties();
                        break;
                    }
                }
            }

            // Fetch the tokens
            string soPath = "Assets/VR-Molecular-Chemistry-Lab/ScriptableObjects/Atoms";
            CreateAtomPrefab("H", AssetDatabase.LoadAssetAtPath<AtomToken>($"{soPath}/H_Token.asset"), targetFolder);
            CreateAtomPrefab("O", AssetDatabase.LoadAssetAtPath<AtomToken>($"{soPath}/O_Token.asset"), targetFolder);
            CreateAtomPrefab("C", AssetDatabase.LoadAssetAtPath<AtomToken>($"{soPath}/C_Token.asset"), targetFolder);
            CreateAtomPrefab("N", AssetDatabase.LoadAssetAtPath<AtomToken>($"{soPath}/N_Token.asset"), targetFolder);

            Debug.Log("[AtomPrefabGenerator] Generated all 4 physical Atom Prefabs ready for grabbing!");
        }

        private static void CreateAtomPrefab(string symbol, AtomToken token, string folderPath)
        {
            if (token == null) return;

            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.name = $"{symbol}_Atom";

            // Physics and Layer
            root.layer = LayerMask.NameToLayer("Atom");
            root.transform.localScale = Vector3.one * 0.12f;

            Rigidbody rb = root.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Renderer
            if (token.atomMaterial != null)
            {
                root.GetComponent<MeshRenderer>().sharedMaterial = token.atomMaterial;
            }

            // XR Interactions
            XRGrabInteractable grab = root.AddComponent<XRGrabInteractable>();
            grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;

            // Atom Controller Logic
            AtomController controller = root.AddComponent<AtomController>();
            controller.atomData = token;
            controller.atomLayer = LayerMask.GetMask("Atom");
            controller.proximityRadius = 0.12f;

            // Optional: Create text label
            GameObject label = new GameObject("SymbolLabel");
            label.transform.SetParent(root.transform);
            TextMesh tm = label.AddComponent<TextMesh>();
            tm.text = symbol;
            tm.characterSize = 0.05f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = symbol == "H" ? Color.black : Color.white; // White text normally, Black for Hydrogen (white sphere)
            
            // Prefab Save
            string prefabPath = $"{folderPath}/{symbol}_Atom.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            GameObject.DestroyImmediate(root);
        }
    }
}
