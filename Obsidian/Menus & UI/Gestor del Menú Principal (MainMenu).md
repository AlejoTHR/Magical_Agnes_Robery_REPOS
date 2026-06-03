Este sistema centraliza el flujo de navegación, la persistencia de las configuraciones de audio y el control de transiciones de pantalla del menú principal. Utiliza un patrón de diseño *Singleton* (`Instance`) para permitir el acceso global desde componentes secundarios de la interfaz.

#### Flujo de Navegación y Paneles
La interfaz se organiza mediante estructuras modulares independientes (*Panels*) que se activan o desactivan según las acciones de usuario, garantizando la compatibilidad total con mandos al reasignar de forma dinámica el foco del `EventSystem`:

* **Estructura de Paneles:** Al iniciar, la escena fuerza el panel principal (`_MainPanel`) como activo y desactiva por defecto las pantallas de opciones, audio y créditos.
* **Control de Enfoque:** Al transicionar entre paneles, se invoca un limpiado del foco actual y se asigna el nuevo elemento inicial para evitar la pérdida de navegación en mandos:

| Panel Origen | Acción de Activación | Panel Destino Activo | Botón Enfocado |
| :--- | :--- | :--- | :--- |
| Inicio de Escena | Automático | `_MainPanel` | `_FirstButtonMain` |
| `_MainPanel` | `OpenOptions()` | `_OptionsPanel` | `_FirstButtonOptions` |
| `_OptionsPanel` | `BackFromOptions()` | `_MainPanel` | `_FirstButtonMain` |
| `_OptionsPanel` | `OpenAudio()` | `_AudioPanel` | `_FirstButtonAudio` |
| `_AudioPanel` | `BackFromAudio()` | `_OptionsPanel` | `_FirstButtonOptions` |
| `_OptionsPanel` | `OpenCredits()` | `_CreditsPanel` | `_FirstButtonCredits` |
| `_CreditsPanel` | `BackFromCredits()` | `_OptionsPanel` | `_FirstButtonOptions` |

#### Secuencia de Transición y Carga de Escena
La carga del juego se procesa de forma asíncrona para evitar cortes visuales abruptos mediante el uso de corrutinas de control:
* **Bloqueo de Entrada:** Al invocar `StartGame()`, una variable de estado (`isTransitioning`) bloquea cualquier acción posterior para evitar llamadas duplicadas.
* **Animación de Entrada/Salida:** El sistema reproduce la animación de ocultar pantalla (`FadeIn`) mediante un componente *Animator*. Tras una espera temporal fija en segundos basada en el valor de `_animDuration`, se ejecuta el cambio definitivo de escena hacia el nivel `"0 - Tutorial"`. Atendiendo al flujo inverso, al iniciar la escena de menús, se reproduce de manera automática la animación de apertura de pantalla (`FadeOut`).

#### Configuración, Conversión y Almacenamiento de Audio
El control de volumen vincula dinámicamente los componentes deslizadores de la interfaz (*Sliders*) con los canales expuestos del *AudioMixer*, procesando el guardado local del progreso a través de la API de `PlayerPrefs`:

* **Inicialización Automatizada:** En el arranque (`Start`), se recuperan los valores almacenados con una base predeterminada de un $75\%$ de intensidad ($0.75\text{f}$). Los deslizadores se enlazan mediante delegados en código para actualizar el mezclador en tiempo real ante cualquier cambio.
* **Conversión a Decibelios (dB):** Dado que la escala de los deslizadores de la UI es lineal (de $0$ a $1$) y la atenuación del sonido es logarítmica, el sistema convierte la métrica lineal a decibelios mediante la fórmula:
  $$\text{dB} = \log_{10}(\max(V_{\text{slider}}, 0.0001)) \times 20$$
  *Nota: Se aplica una sujeción inferior de $0.0001$ para impedir una indeterminación matemática de logaritmo de cero al silenciar por completo el canal.*

* **Canales de Audio Propios:** El componente inicializa por código dos canales de audio dedicados. El canal de interacción de pulsación (`_clickChannel`) se configura con la máxima prioridad de ejecución ($0$) y un temporizador de apagado automático de $0.4$ segundos para garantizar que el sonido global de *click* se escuche con limpieza y sin interrupciones por saturación.