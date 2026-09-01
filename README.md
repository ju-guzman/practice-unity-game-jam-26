# Cempasuchil

> *"La musica que guia a las almas de vuelta a casa"*

Point & Click atmosferico en 2D desarrollado durante una Game Jam con Unity. El jugador despierta sin recuerdos en una habitacion compartida por dos hermanas gemelas y debe explorarla guiandose por el sonido: la musica sube cuando el cursor (una linterna) se acerca a objetos narrativos, y baja si el jugador tarda demasiado en encontrarlos. Encontrar los cuatro objetos en el orden correcto reconstruye la historia de las hermanas; perderlos hace que la habitacion se apague y silencie.

## Estado del proyecto

**ALPHA 0.1** — build jugable de principio a fin (pantalla de inicio, gameplay, final), producto de 4 sesiones de desarrollo durante la jam. El diseno completo esta documentado en el GDD; esta alpha implementa una version simplificada y ajustada al tiempo de la jam.

## Gameplay implementado

- **Linterna como cursor:** un `Light2D` de URP sigue al mouse con suavizado (`MouseFlashlightController`), iluminando la habitacion en penumbra.
- **Deteccion por proximidad:** un cursor de interaccion (`InteractionCursor2D`) detecta objetos coleccionables bajo el mouse mediante `Physics2D.OverlapPoint` sobre una capa dedicada.
- **Recoleccion en secuencia estricta:** los objetos narrativos (`CollectableItem`) solo son interactuables cuando `GameManager` los desbloquea en el orden configurado; recolectar el correcto suma tiempo y desbloquea el siguiente.
- **Tension como valor unico (0–1):** en vez de timers independientes por objeto, la version implementada usa un unico contador regresivo (`GameManager.Value`) que baja con el tiempo y sube al recolectar objetos. Ese valor conduce **todo** el feedback sensorial del juego:
  - **Musica adaptativa por stems** (`MusicTensionController`): activa/desactiva capas musicales y aplica low-pass/distorsion segun la tension, vía Audio Mixer.
  - **Iluminacion y post-procesado** (`LightingTensionController` + `PostProcessLightingController`): interpola intensidad ambiental, vignette, bloom, saturacion y color filter.
  - **Feedback visual de UI** (`TensionSpriteFader`, `CollectionProgressSpriteFader`): sprites que se desvanecen segun tension y progreso de recoleccion.
- **Decadencia ambiental aleatoria** (`RandomItemFadeSystem`): cada cierto intervalo, un objeto decorativo activo se desvanece, degradando visualmente la habitacion sin patrones predecibles.
- **Navegacion entre vistas de la habitacion:** 4 escenas (`RoomView1`–`RoomView4`) se cargan de forma aditiva y posicionan sus objetos en el mundo; `RoomCameraNavigator` desplaza la camara entre ellas con fundidos, en input dedicado (siguiente/anterior).
- **Reinicio de bucle:** al llegar a silencio total la musica se detiene, se dispara la pantalla final, y el jugador puede reiniciar el nivel (fade + recarga de escena).
- **UI 100% en UI Toolkit** (UXML/USS) para pantallas de inicio y final, sin Canvas/UGUI.

## Controles

| Accion                                  | Input                               |
| --------------------------------------- | ----------------------------------- |
| Mover linterna / apuntar                | Mouse                               |
| Interactuar / recolectar                | Clic izquierdo                      |
| Cambiar de vista (siguiente / anterior) | Acciones dedicadas del Input System |
| Reiniciar (tras Game Over)              | Accion dedicada del Input System    |

Implementado con el **Input System** de Unity (`InputSystem_Actions.inputactions` + `InputActions.cs` generado), expuesto a todo el juego a traves del singleton `InputManager`.

## Arquitectura tecnica

- **Motor:** Unity `6000.4.10f1`, Universal Render Pipeline (2D Renderer).
- **Plataforma objetivo:** Web (WebGL) — perfiles de build `Web - Desktop - Development` / `Web - Desktop - Release`.
- **Audio:** `AudioSource`/`AudioMixer` nativos de Unity (se evito Wwise por compatibilidad con WebGL).

```
Assets/Scripts/
├── Audio/       # Config y controlador de stems musicales dinamicos
├── Core/        # GameManager (estado, timer, secuencia de coleccion), navegacion de camara
├── Inputs/      # Input System wrapper (singleton InputManager)
├── Interaction/ # Objetos coleccionables, cursor de interaccion, fade ambiental
├── Lighting/    # Linterna, iluminacion y post-proceso guiados por tension
├── UI/          # Pantallas de inicio/fin y HUD (UI Toolkit)
├── Utils/       # Helpers (screen-to-world)
└── Visual/      # Sprites reactivos a tension/progreso
```

La escena `Game` actua como orquestador: carga aditivamente las 4 escenas `RoomView*`, ubica cada una segun un offset configurado en `GameManager`, y localiza el punto de camara de cada sala por tag (`CameraView`) para que `RoomCameraNavigator` pueda moverse entre ellas.

## Historia (resumen)

El jugador es una de dos hermanas gemelas, atrapada en un limbo con forma de su habitacion de la infancia. Cuatro objetos narrativos —la cobija, el libro, el control roto y las pastillas— trazan, en ese orden, el arco de su relacion: nacimiento, crecimiento, conflicto y la enfermedad/muerte de una de ellas. El color naranja de la flor de Cempasuchil (simbolo del Dia de Muertos que guia a las almas de vuelta a casa) marca visualmente el camino a lo largo del juego. Los detalles completos de personajes, objetos y storyboard final estan en el [GDD](Cempasuchil_GDD_v1.html).

## Diferencias con el GDD de diseño original

El GDD describe la vision completa del juego; por el alcance de la jam, la implementacion actual simplifica algunos sistemas:

- Un **unico valor de tension global** reemplaza los timers independientes por objeto y las dos capas de volumen (base + proximidad) descritas en el documento.
- La **navegacion entre salas** se dispara con una accion de input dedicada en vez de mover el cursor a los bordes de la pantalla.
- La **decadencia del entorno** es aleatoria por intervalos, no organizada en las 4 capas de prioridad fijas del GDD.
- La cinematica final y el diario ilustrado de 5 paginas descritos en el GDD se resuelven en esta alpha con una pantalla final (`EndScreenController`) y una secuencia de sprites.

## Créditos

Proyecto desarrollado en equipo durante la Amix Game Jam 2026 por:

Andre García Cristancho (Diseñador)

Paula Alejandra Hernández González (Producción)

Julián Mateo Guzmán Alba (Programador)

[Itch.io](https://mgs-andregarcia.itch.io/cempasuchil)
