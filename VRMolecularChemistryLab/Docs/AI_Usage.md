# 🤖 Project AI Usage Documentation

This project actively leveraged several Artificial Intelligence tools to accelerate backend development, structure complex XR interaction logic, and automate UI asset creation. The workflow heavily combined large language models with editor-based coding assistants.

## 🛠️ Main AI Tools Leveraged

- **Antigravity AI / Claude**  
  Used persistently during development as the primary engineering co-pilot. Assisted with architectural ideation, solving complex Unity XR interaction bugs, and safely migrating the project from simple proximity checks to the mathematically robust `BondSocketManager` array system. Also refined the overall documentation and UI behaviors.

- **GitHub Copilot & VS Code Agent**  
  Used extensively for localized real-time code completion, boilerplate generation, C# syntax prediction, and context-aware architectural lookup queries directly inside the IDE.

- **ChatGPT**  
  Used for high-level system design, feature planning, drafting onboarding instructions, and mapping out the `MoleculeDatabase` data structures.

- **DALL·E**  
  Used to generate UI visuals, atomic discovery icons, instructional sprite sheets, and stylized intro imagery—drastically reducing the time spent on manual 2D asset iteration.

- **Voicemaker**  
  Enabled the generation of clear, instructional voice-over audio for immersive tutorial feedback without the need for manual recording.

---

## 🧪 Focus Case Study: Molecule Inspector Module

A significant portion of the AI collaboration was dedicated to building the **Molecule Inspector System**.

### Example Use Cases:
* Designed the `MoleculeDetailsUI` as a globally accessible Singleton to seamlessly connect scene-level Canvas interfaces with spawned VR prefabs.
* Structured the `BondType` variables and nested `MoleculeData` data models.
* Optimized the XR grab lifecycle (intercepting `selectEntered`/`selectExited` gracefully without hard-disabling colliders during the bond formation).
* Drafted C# string formatting algorithms to automatically convert plain text formulas like `H2O` into proper chemical subscript elements (H₂O) for TextMeshPro.

### Sample Prompt used in Development:
> “Design a Molecule Inspector system in Unity XR that shows molecule name, formula, and bonds when user grabs it. create a script for molecule detalis to show . when the molecule is spawned and user grabed it , show this UI with data like name , formula , bond type and sicription of this molecule .. need a fade in and out effect.”
