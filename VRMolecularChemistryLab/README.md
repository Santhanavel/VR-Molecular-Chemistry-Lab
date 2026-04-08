# VR Molecular Chemistry Lab 🧪 (Meta Quest 3)

Welcome to the VR Molecular Chemistry Lab project! This is an immersive virtual reality educational experience designed for the Meta Quest, built using Unity and the XR Interaction Toolkit.

## 🌟 Overview
The VR Chemistry Lab provides a fully interactive sandbox environment where users can learn about atomic scale chemistry. Users can pick up foundational raw atoms using hand-tracking or controllers, place them into interactive chemical bond sockets, and physically construct up to 18 distinct molecular structures! Built with a clean, dynamic codebase, the lab evaluates real-time atomic formations and scales successful bond chains into massive 3D models.

## 🚀 Features
* 🔬 **Atom creation and manipulation**
* 🔗 **Real-time bond formation system** dynamically evaluating sequences.
* 🧠 **Molecule validation system** verifying accurate chemical structures.
* 📊 **Interactive Molecule Inspector** displaying crucial structure details.
* 🎧 **Audio feedback system** for successful and failed constructions.
* 🥽 **Fully functional in VR** (Meta Quest 2 / 3) using hand-tracking and controllers.

## 🧪 Molecule Inspector
When the user **grabs or examines a molecule**, a dynamic panel appears showing:
* **Molecule Name:** (e.g. Water)
* **Chemical Formula:** (e.g. H₂O)
* **Bond Information:** Type (Single/Double/Triple) and connections.

## 🎮 VR Controls
* **Grab** → Pick up raw atoms / completed molecules to inspect or move.
* **Release** → Drop atoms to socket them or reset them; drop molecules to automatically return them to the viewing station.
* **Inspect** → Grabbing a completed molecule scales it up and dynamically fades in the Molecule Inspector panel.

## ⚙️ Setup Instructions
1. Open the project in Unity (Optimized for **Unity 6000.x**).
2. Ensure the following dependencies are installed via Package Manager:
   * **XR Interaction Toolkit** (2.x+)
   * **XR Plugin Management**
3. Enable **OpenXR** in your Project Settings.
4. Load the main scene:
   `Assets/Scenes/MainScene.unity`
5. Build & Run on your **Meta Quest**.

## 🛠️ Workflow & Architecture
Our development workflow focused heavily on converting static interactions into a highly robust, dynamic system specifically optimized for Quest hardware performance:
1. **System Migration**: Upgraded from proximity-based physics bindings to a secure **socket-based architecture** (`BondSocket.cs`, `BondSocketManager.cs`).
2. **Dynamic Spawning**: Engineered a responsive system that dynamically generates holographic placement nodes according to mathematically validated chemical combinations.
3. **Interactive Physics**: Programmed an intuitive VR interaction layer where atoms smoothly hover (`AntigravityFloat.cs`), intelligently auto-sort themselves into element groups, and cleanly snap back to their anchors when released or rejected.
4. **Data-Driven Design**: Unified all interaction rules via a centralized ScriptableObject infrastructure (`MoleculeDatabase.asset`), easily managing large complex chains like Glycine.
5. **Seamless Session Loops**: Built soft-reset tools (`GameStateManager.cs`) and UI library discovery logic to maintain a completely unbroken user experience.

## 📂 Key Project Structure
* `Scripts/`
  * `Core/AtomController.cs`
  * `Core/BondSocketManager.cs`
  * `Core/MoleculeSpawnController.cs`
  * `UI/MoleculeDetailsUI.cs` (Inspector)
  * `Data/MoleculeData.cs`
* `Prefabs/` (Interactable atoms, Bond chains, Molecule models)

## 🎥 Demo
👉 *(Add your VR recording video link here)*

## ✅ Status
✔ Core features and architecture completed
✔ VR interactions, grabbing, and physics layers perfectly stabilized
✔ Molecule Inspector and responsive UI implemented

---
**Note on AI Usage:** Development of this VR Lab greatly leveraged generative AI tools. For detailed documentation on tools, processes, and sample prompt logs, please refer to: [Docs/AI_Usage.md](./Docs/AI_Usage.md).
