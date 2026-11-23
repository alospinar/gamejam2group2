# GameJam2 Group2

**[English](#english) | [Español](#español)**

## English

### About

**GameJam2 Group2** is a 2D side-scrolling game created as part of a **GameJam2**. The player explores three levels, collects tickets, and uses mushrooms to switch between a real world and a fantasy world with different visuals, platforms, colliders, and physics states.

The game includes anxiety and mushroom-management mechanics, police detection, NPC dialogue, pushable furniture, a pause menu, settings, credits, victory, and two types of game over.

- **Play:** [itch.io - El viaje de Paco](https://wooliesam.itch.io/el-viaje-de-paco)
- **GDD:** [Game Design Document](https://drive.google.com/file/d/1GtLMmGCBQ7XVUMhkAWjpg1ZomoxjptDr/view?usp=sharing)

### Controls

| Action | Keyboard / Mouse | Gamepad |
| --- | --- | --- |
| Move | `A` / `D` or Left / Right arrows | Left stick or D-pad |
| Climb | `W` / `S` or Up / Down arrows | Left stick |
| Jump | `Space` | South button |
| Interact / Talk / Push | Hold `E` | North button |
| Switch worlds | `Q` | Not currently bound |
| Pause / Resume | `Esc` | --- |

- Mushrooms provide charges for entering the fantasy world.
- Entering the fantasy world consumes one mushroom charge.
- NPC and furniture interactions begin with `E`.
- Press `E` again while dialogue is open to advance the text.
- Hold `E` and move toward a piece of furniture to push it.

### Objective

1. Explore each level.
2. Collect mushrooms to gain world-switching charges.
3. Collect the tickets required to complete the journey.
4. Reach the exit of each level.
5. Avoid dangerous areas and police vision to keep the anxiety meter from filling and avoid being detected.
6. Complete the final level and reach the victory screen.

### Gameplay Systems

- **World switching:** swaps sprites, world scales, colliders, rigidbody states, active tilemaps, and music.
- **Anxiety:** danger zones increase anxiety over time; reaching the limit ends the run.
- **Police detection:** police cones detect the player and trigger a game-over state.
- **Tickets:** collected across the level sequence and required for final completion.
- **Mushrooms:** collectible charges used to enter the fantasy world.
- **NPC dialogue:** nearby NPCs can be talked to through the interaction system.
- **Pushable furniture:** interactive objects can be moved horizontally while the player is in range and holding `E`.
- **Menus and pause:** title, settings, credits, pause, victory, and game-over screens.
- **Persistence:** the level manager keeps the session's anxiety, mushrooms, tickets, and game state across scenes.

### Areas for Improvement

- Adjust the animation speeds so character and gameplay animations look smoother and more fluid.
- Complete the pause-menu UI with all the required buttons and states.
- Adapt the UI size and layout for different screen resolutions and aspect ratios.
- Correct and refine existing colliders.
- Adjust the camera size to improve framing and visibility across the levels.

### Project Setup

#### Requirements

- **Unity 6000.3.24f1**
- Unity modules:
  - 2D Animation
  - 2D Tilemap Extras
  - Cinemachine
  - URP
  - Input System
  - TextMesh Pro

#### Run the Project

1. Open Unity Hub.
2. Add this repository or open the project folder directly.
3. Open the project with **Unity 6000.3.24f1**.
4. Open `Assets/Scenes/Pantalla Inicial.unity`.
5. Press **Play** in the Unity Editor.

The configured build scene order is:

1. `Pantalla Inicial`
2. `Level 1`
3. `Level 2`
4. `Level 3`
5. `Derrota 1`
6. `Derrota 2`
7. `Victoria`

### Main Code Structure

- `Assets/Scripts/PlayerController.cs` - movement, jumping, ladder climbing, gravity, and animations.
- `Assets/Scripts/WorldManager.cs` - real/fantasy world switching and charges.
- `Assets/Scripts/WorldObject.cs` - sprites, colliders, scales, and rigidbodies for switched worlds.
- `Assets/Scripts/LevelManager.cs` - persistent session state, tickets, mushrooms, anxiety, and level transitions.
- `Assets/Scripts/DangerAnxiety.cs` - anxiety increase in danger areas.
- `Assets/Scripts/PoliceVision.cs` - police field-of-view detection.
- `Assets/Scripts/Interaction/` - interaction interface, NPC dialogue, player interaction, and pushable furniture.
- `Assets/Scripts/LevelExit.cs` - level completion triggers.
- `Assets/Scripts/MushroomPickup.cs` - mushroom charge pickups.
- `Assets/Scripts/TickectPickup.cs` - ticket pickups.
- `Assets/Scripts/Parallax.cs` - camera-based parallax movement.
- `Assets/Scenes/AnaScene/Scripts/GameManager.cs` - menus, HUD, pause, settings, and game-over UI.
- `Assets/Scenes/AnaScene/Scripts/SoundManager.cs` - music, SFX, and volume controls.
- `Assets/InputSystem_Actions.inputactions` - keyboard and gamepad input bindings.

### Team

**Andres Leonardo Ospina Rios**

*Project Manager & Gameplay Programmer*

**Dhana Samanthan Aragon Lopez**

*2D Artist, UI Designer & Animator*

**Ana Maria Rohini Sakti Rodriguez Correa**

*Level Designer, UI Designer & SFX*

**Nerlyn Juliet Carlosama Molina**

*Documentation & Pitch Lead*

---

## Español

### Acerca del juego

**GameJam2 Group2** es un juego 2D de desplazamiento lateral creado como parte de una **GameJam2**. El jugador recorre tres niveles, recoge boletos y usa hongos para alternar entre un mundo real y un mundo fantástico con diferentes visuales, plataformas, colisionadores y estados físicos.

El juego incluye mecanicas de ansiedad y gestion de hongos, deteccion policial, dialogos con NPC, muebles empujables, pausa, ajustes, creditos, victoria y dos tipos de derrota.

- **Jugar:** [itch.io - El viaje de Paco](https://wooliesam.itch.io/el-viaje-de-paco)
- **GDD:** [Documento de diseño del juego](https://drive.google.com/file/d/1GtLMmGCBQ7XVUMhkAWjpg1ZomoxjptDr/view?usp=sharing)

### Controles

| Accion | Teclado / Raton | Mando |
| --- | --- | --- |
| Moverse | `A` / `D` o Flechas izquierda / derecha | Stick izquierdo o cruceta |
| Escalar | `W` / `S` o Flechas arriba / abajo | Stick izquierdo |
| Saltar | `Espacio` | Boton sur |
| Interactuar / Hablar / Empujar | Mantener `E` | Botón norte |
| Cambiar de mundo | `Q` | No asignado actualmente |
| Pausar / Reanudar | `Esc` | --- |

- Los hongos proporcionan cargas para entrar al mundo fantástico.
- Entrar al mundo fantástico consume una carga de hongo.
- Las interacciones con NPC y muebles comienzan con `E`.
- Cuando el dialogo esta abierto, pulsa `E` nuevamente para avanzar el texto.
- Mantén `E` y mueve al personaje hacia un mueble para empujarlo.

### Objetivo

1. Explorar cada nivel.
2. Recoger hongos para obtener cargas de cambio de mundo.
3. Recoger los boletos necesarios para completar el recorrido.
4. Llegar a la salida de cada nivel.
5. Evitar las zonas peligrosas y el campo de vision policial para no llenar la barra de ansiedad ni ser detectado.
6. Completar el nivel final y llegar a la pantalla de victoria.

### Sistemas de juego

- **Cambio de mundo:** cambia sprites, escalas, colisionadores, estados de rigidbody, tilemaps activos y música.
- **Ansiedad:** las zonas peligrosas aumentan la ansiedad con el tiempo; al alcanzar el limite termina la partida.
- **Deteccion policial:** los conos de vision policial detectan al jugador y activan la derrota.
- **Boletos:** se recolectan durante los niveles y son necesarios para completar el juego.
- **Hongos:** objetos recolectables que se usan para entrar al mundo fantástico.
- **Dialogos con NPC:** los personajes cercanos pueden ser contactados mediante el sistema de interaccion.
- **Muebles empujables:** algunos objetos interactivos pueden moverse horizontalmente mientras el jugador está cerca y mantiene `E`.
- **Menús y pausa:** pantalla inicial, ajustes, créditos, pausa, victoria y derrotas.
- **Persistencia:** el administrador de niveles conserva la ansiedad, los hongos, los boletos y el estado de juego entre escenas.

### Pendientes por mejorar

- Ajustar la velocidad de las animaciones para que los movimientos del personaje y las animaciones del juego sean más fluidos.
- Completar la interfaz del menú de pausa con los botones y estados necesarios.
- Ajustar el tamaño y la distribución de la interfaz para que funcione en diferentes tamaños de pantalla y resoluciones.
- Corregir y ajustar algunos colliders que necesitan mejoras.
- Ajustar el tamaño de la cámara para mejorar el encuadre y la visibilidad en los niveles.

### Configuración del proyecto

#### Requisitos

- **Unity 6000.3.24f1**
- Modulos de Unity:
  - 2D Animation
  - 2D Tilemap Extras
  - Cinemachine
  - URP
  - Input System
  - TextMesh Pro

#### Ejecutar el proyecto

1. Abrir Unity Hub.
2. Agregar este repositorio o abrir directamente la carpeta del proyecto.
3. Abrir el proyecto con **Unity 6000.3.24f1**.
4. Abrir `Assets/Scenes/Pantalla Inicial.unity`.
5. Pulsar **Play** en el Editor de Unity.

El orden configurado de escenas de compilación es:

1. `Pantalla Inicial`
2. `Level 1`
3. `Level 2`
4. `Level 3`
5. `Derrota 1`
6. `Derrota 2`
7. `Victoria`

### Estructura principal del código

- `Assets/Scripts/PlayerController.cs` - movimiento, salto, escalado, gravedad y animaciones.
- `Assets/Scripts/WorldManager.cs` - cambio entre mundo real y mundo fantástico y gestión de cargas.
- `Assets/Scripts/WorldObject.cs` - sprites, colisionadores, escalas y rigidbodies de los mundos.
- `Assets/Scripts/LevelManager.cs` - estado persistente, boletos, hongos, ansiedad y transiciones.
- `Assets/Scripts/DangerAnxiety.cs` - aumento de ansiedad en zonas peligrosas.
- `Assets/Scripts/PoliceVision.cs` - deteccion policial mediante campo de vision.
- `Assets/Scripts/Interaction/` - interfaz de interaccion, dialogos, interaccion del jugador y muebles empujables.
- `Assets/Scripts/LevelExit.cs` - activadores de finalizacion de nivel.
- `Assets/Scripts/MushroomPickup.cs` - recogida de cargas de hongo.
- `Assets/Scripts/TickectPickup.cs` - recogida de boletos.
- `Assets/Scripts/Parallax.cs` - movimiento parallax basado en la camara.
- `Assets/Scenes/AnaScene/Scripts/GameManager.cs` - menus, HUD, pausa, ajustes y pantallas de derrota.
- `Assets/Scenes/AnaScene/Scripts/SoundManager.cs` - musica, efectos y control de volumen.
- `Assets/InputSystem_Actions.inputactions` - asignaciones de teclado y mando.

### Integrantes

**Andres Leonardo Ospina Rios**

*Project Manager & Gameplay Programmer*

**Dhana Samanthan Aragon Lopez**

*2D Artist, UI Designer & Animator*

**Ana Maria Rohini Sakti Rodriguez Correa**

*Level Designer, UI Designer & SFX*

**Nerlyn Juliet Carlosama Molina**

*Documentation & Pitch Lead*
