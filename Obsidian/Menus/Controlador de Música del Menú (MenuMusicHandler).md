Este componente se encarga de gestionar la reproducción continua de la banda sonora del menú principal y su posterior transición auditiva atenuada al cambiar hacia las escenas jugables. Garantiza que la música no se corte de manera abrupta al iniciar la partida.

#### Persistencia entre Escenas y Ciclo de Vida
El sistema requiere de forma obligatoria un componente `AudioSource` en el mismo objeto (`[RequireComponent]`) y utiliza delegados del motor para monitorizar el flujo de pantallas:

* **Persistencia Automatizada:** Durante la fase de inicialización (`Awake`), el objeto se configura mediante `DontDestroyOnLoad(gameObject)`. Esto evita que el motor destruya el canal de audio al descargar el menú principal, permitiendo que la pista siga sonando en la pantalla de carga o en los primeros instantes del tutorial.
* **Suscripción a Eventos:** En el arranque (`Start`), el script se suscribe al evento global de carga del motor (`SceneManager.sceneLoaded`). Para evitar fugas de memoria o referencias muertas, el componente se desvincula automáticamente de este evento (`SceneManager.sceneLoaded -= OnSceneLoaded`) en su método de destrucción (`OnDestroy`).

#### Transición de Salida Discreta (Fade Out)
El desvanecimiento de la pista musical se gestiona mediante una corrutina en base al índice de compilación de las escenas (*Build Index*):

* **Detección de Cambio:** Cada vez que se completa la carga de un nivel, se ejecuta de forma automática el método `OnSceneLoaded`. Si el índice de la escena activa es distinto de `0` (el cual corresponde al Menú Principal) y el sistema no está en proceso de apagado, se inicia la secuencia de salida.
* **Atenuación Lineal Temporal:** La intensidad del volumen se degrada de manera fluida frame a frame mediante una interpolación lineal (`Lerp`) insensible a los picos de carga:
  $$V_{\text{audio}} = \text{Lerp}(V_{\text{inicial}}, 0, \frac{t}{\text{fadeDuration}})$$
* **Ciclo de Limpieza:** Una vez que el tiempo transcurrido ($t$) alcanza la duración establecida en segundos (`fadeDuration`), el volumen se fuerza a $0$, se detiene el canal de reproducción de forma definitiva y se invoca `Destroy(gameObject)` para liberar el espacio en memoria del objeto persistente.