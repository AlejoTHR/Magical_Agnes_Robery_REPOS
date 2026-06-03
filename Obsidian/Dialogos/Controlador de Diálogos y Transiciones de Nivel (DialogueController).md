Este componente gestiona las interacciones narrativas del juego, controlando el despliegue de secuencias de texto en la interfaz de usuario (UI), la renderización de retratos de personajes y los cambios automáticos de nivel o de salas. El script está diseñado de forma versátil para operar bajo dos configuraciones estructurales: carteles informativos (`isSignMode`) o cinemáticas conversacionales con Personajes No Jugadores (NPCs).

#### Estructuras de Datos de la Secuencia (`DialogueLine`)
El sistema encapsula cada línea de diálogo de forma secuencial en objetos estructurados serializables (`DialogueLine`), exponiendo en el Inspector de Unity los siguientes campos:
* **Identificadores Visuales:** `characterName` (nombre expuesto en la caja de texto), `characterPortrait` (sprite del rostro de la entidad hablante) y `backgroundOverride` (asset gráfico opcional para modificar el fondo de la interfaz).
* **Parámetros de Flujo:** `lineSpeed` (control de tiempo fraccionado para el efecto de máquina de escribir) y el booleano `triggersLevelTransition`, que marca de manera específica si dicha línea debe clausurar el entorno actual al finalizar.

#### Máquina de Estados e Interrupción de Lectura
Durante el ciclo `Update`, el script evalúa el estado del flujo conversacional interactuando directamente con el mapa de entradas (`PlayerInput`) del personaje:
1. **Validación de Avance:** Si la bandera `isDialogueActive` es verdadera y el jugador presiona la acción `"Interact"`, el sistema realiza una bifurcación de control:
   * Si el texto se está renderizando dinámicamente (`isTyping = true`), se invoca `FinishLineInstantly()`, lo que detiene la corrutina y plasma el bloque de texto completo de forma inmediata.
   * Si la línea ha terminado de renderizarse, se invoca `AdvanceOrEnd()` para transicionar al siguiente índice de la lista o clausurar la secuencia.
2. **Disparo por Proximidad:** Si el diálogo no está activo pero Agnes entra en el radio geométrico del disparador (`isPlayerInRange`) y se encuentra desactivada la propiedad automática `playOnEnter`, presionar la tecla de interacción arranca el método `StartDialogueSequence()`.

#### Bloqueo Cinemático de Controles (TogglePlayerControls)
Al iniciar una interacción conversacional estándar (donde `isSignMode` sea falso), el script deshabilita la locomoción de Agnes para evitar que el jugador se desplace durante la lectura:
* **Frenado y Congelación Física:** Invoca el script de movimiento, anula las fuerzas residuales cinemáticas del cuerpo rígido e inyecta una máscara de restricciones físicas absolutas:
  $$\vec{V}_{\text{player.rb}} = \vec{0}$$
  $$\text{Constraints} = \text{FreezeAll}$$
  Además, conmuta de forma interna la bandera `playerMovementScript.isHiding` como directiva de seguridad para mitigar alertas con las IA enemigas durante el evento narrativo.
* **Restauración de Libertad:** Al finalizar el diálogo, devuelve el control a la locomoción base y limpia las restricciones de posición física del `Rigidbody2D`, preservando únicamente el bloqueo estándar de rotación sobre el eje z (`FreezeRotation`).

#### Renderizado Dinámico y Efecto Máquina de Escribir (TypeEffect)
El procesamiento del texto de forma progresiva se gestiona mediante el método asíncrono `DisplayLine()` y la corrutina `TypeEffect`:
* **Modulación del Perfil Visual:** Si opera en modo cartel (`isSignMode`), desactiva de forma automática las imágenes de fondo y los retratos. En modo conversacional, evalúa la existencia de assets en `characterPortrait` o `backgroundOverride` para activar o apagar los componentes tipográficos correspondientes en la interfaz de usuario.
* **Rutina de Volcado:** Limpia el contenedor de texto (`dialogueText.text = ""`). Si la velocidad configurada es menor o igual a cero, imprime la cadena instantáneamente; en caso contrario, almacena la corrutina en la variable de control `currentTypewriter` para volcar los caracteres uno a uno espaciados por la tasa de refresco inyectada en `lineSpeed`:
  $$\text{Intervalo de Espera} = \text{WaitForSeconds}(\text{lineSpeed})$$

#### Sistema Dual de Transición de Escenarios (ExecuteTransition)
Al alcanzar el final de una secuencia tipográfica con la bandera `triggersLevelTransition` activa, el script clausura el panel UI e invoca la lógica de carga en base al enumerado paramétrico `TransitionType`:

```
   /---> [LoadNextRoomPrefab]  --> LevelManager.Instance.LoadNextRoom()
[Dialogue End] --> ExecuteTransition() 
   \---> [LoadSpecificScene]   --> SceneManager.LoadScene(sceneToLoad)
```           
***1. Modo `LoadNextRoomPrefab`:** Diseñado para estructuras modulares basadas en prefabs dentro de una misma escena técnica. Invoca de forma segura al singleton `LevelManager.Instance.LoadNextRoom()` para permutar las habitaciones físicas y restablece los controles del jugador inmediatamente dado que no existe cambio de contexto en el motor.
 **2. Modo `LoadSpecificScene`:** Diseñado para transiciones de gran envergadura. Valida que la cadena de texto `sceneToLoad` contenga un nombre válido y delega el control en el gestor de escenas nativo de Unity mediante `SceneManager.LoadScene()`.