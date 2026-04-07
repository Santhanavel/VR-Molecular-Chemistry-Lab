using UnityEditor;
using UnityEngine;
using VRMolecularLab.Data;

namespace VRMolecularLab.Editor
{
    public class AtomTokenGenerator
    {
        [MenuItem("VRLab/Generate Atom Tokens & Materials")]
        public static void GenerateTokens()
        {
            string rootPath = "Assets/VR-Molecular-Chemistry-Lab";
            string matFolder = rootPath + "/Materials";
            string matAtomsFolder = matFolder + "/Atoms";
            string soFolder = rootPath + "/ScriptableObjects";
            string soAtomsFolder = soFolder + "/Atoms";

            // Ensure directories exist
            EnsureFolderExists(rootPath, "Materials");
            EnsureFolderExists(matFolder, "Atoms");
            EnsureFolderExists(rootPath, "ScriptableObjects");
            EnsureFolderExists(soFolder, "Atoms");

            // Element definitions based on DOC-04 CPK Standard
            CreateTokenAndMaterial(soAtomsFolder, matAtomsFolder, "H", "Hydrogen", new Color(1f, 1f, 1f)); // #FFFFFF
            CreateTokenAndMaterial(soAtomsFolder, matAtomsFolder, "O", "Oxygen", new Color(1f, 0.125f, 0.125f)); // #FF2020 approximation
            CreateTokenAndMaterial(soAtomsFolder, matAtomsFolder, "C", "Carbon", new Color(0.25f, 0.25f, 0.25f)); // #404040 approximation
            CreateTokenAndMaterial(soAtomsFolder, matAtomsFolder, "N", "Nitrogen", new Color(0.251f, 0.5f, 1f)); // #4080FF approximation

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AtomTokenGenerator] Created all 4 Atom Tokens and Materials successfully!");
        }

        private static void CreateTokenAndMaterial(string soFolder, string matFolder, string symbol, string name, Color cpkColor)
        {
            string matPath = $"{matFolder}/Atom_{symbol}_Mat.mat";
            string soPath = $"{soFolder}/{symbol}_Token.asset";

            // 1. Create or load Material
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                // Attempt to use URP Lit, fallback to standard if URP missing
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");

                mat = new Material(shader);
                mat.color = cpkColor;

                // URP specific color assignment mapping
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", cpkColor);
                }

                AssetDatabase.CreateAsset(mat, matPath);
            }

            // 2. Create or load ScriptableObject Token
            AtomToken token = AssetDatabase.LoadAssetAtPath<AtomToken>(soPath);
            if (token == null)
            {
                token = ScriptableObject.CreateInstance<AtomToken>();
                token.elementSymbol = symbol;
                token.elementName = name;
                token.atomColor = cpkColor;
                token.labelText = symbol;
                token.atomMaterial = mat;
                
                // For mesh, ideally load Unity's default sphere mesh
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Mesh sphereMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
                token.atomMesh = sphereMesh;
                GameObject.DestroyImmediate(primitive);

                AssetDatabase.CreateAsset(token, soPath);
            }
            else
            {
                // Update existing
                token.elementSymbol = symbol;
                token.elementName = name;
                token.atomColor = cpkColor;
                token.labelText = symbol;
                token.atomMaterial = mat;
                EditorUtility.SetDirty(token);
            }
        }

        private static void EnsureFolderExists(string parentFolder, string newFolderName)
        {
            if (!AssetDatabase.IsValidFolder($"{parentFolder}/{newFolderName}"))
            {
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }
    }
}
