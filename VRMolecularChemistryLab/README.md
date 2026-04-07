# VR Molecular Chemistry Lab

Welcome to the VR Molecular Chemistry Lab project! This is an immersive virtual reality educational experience designed for the Meta Quest, built using Unity 6000.3.7f1 and the XR Interaction Toolkit.

## Overview
The VR Chemistry Lab provides a fully interactive sandbox environment where users can learn about atomic scale chemistry. Users can pick up foundational raw atoms using hand-tracking or controllers, place them into interactive chemical bond sockets, and physically construct up to 18 distinct molecular structures! Built with a clean, dynamic codebase, the lab evaluates real-time atomic formations and scales successful bond chains into massive 3D models.

## Workflow & Architecture
Our development workflow focused heavily on converting static interactions into a highly robust, dynamic system specifically optimized for Quest hardware performance:

1. **System Migration**: Upgraded from proximity-based physics bindings to a secure **socket-based architecture** (`BondSocket.cs`, `BondSocketManager.cs`).
2. **Dynamic Spawning**: Engineered a responsive system that dynamically generates holographic placement nodes according to mathematically validated chemical combinations.
3. **Interactive Physics**: Programmed an intuitive VR interaction layer where atoms smoothly hover (`AntigravityFloat.cs`), intelligently auto-sort themselves into element groups, and cleanly snap back to their anchors when released or rejected.
4. **Data-Driven Design**: Unified all interaction rules via a centralized ScriptableObject infrastructure (`MoleculeDatabase.asset`), easily managing large complex chains like Glycine.
5. **Seamless Session Loops**: Built soft-reset tools (`GameStateManager.cs`) and UI library discovery logic to maintain a completely unbroken user experience.

---

## 🤖 AI Tools Used

AI tools were heavily leveraged to accelerate development, improve content quality, and streamline workflows:

- **GitHub Copilot**  
Used extensively for real-time code completion, syntax prediction, and generating standard Unity boilerplate to accelerate backend script creation.

- **VS Code Agent**  
Assisted directly inside the desktop IDE environment for localized code generation, refactoring assistance, and context-aware architectural lookup queries.

- **ChatGPT**  
Used for generating and refining **UI content, onboarding instructions, and interaction flow descriptions**, ensuring clarity and consistency in the user experience.

- **DALL·E**  
Used to create **UI visuals, instructional sprite sheets, and welcome/intro imagery**, significantly reducing the need for manual design and iteration.

- **Claude**  
Assisted in **prompt structuring and documentation writing**, helping organize development notes and substantially improve README clarity.

- **Antigravity AI**  
Used persistently during development for **workflow acceleration, architectural ideation, and problem-solving support**. Played a crucial role in safely migrating the project to the complex, physics-safe socket architecture.

- **Voicemaker**  
Used to generate **voice-over audio for in-app instructions**, greatly enhancing tutorial immersion and physical accessibility.
