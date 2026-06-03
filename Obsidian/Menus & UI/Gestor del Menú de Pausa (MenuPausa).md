Este componente administra el sistema de pausa del juego, permitiendo suspender temporalmente la simulación, navegar entre los distintos paneles de la interfaz de pausa y gestionar determinados parámetros de configuración accesibles durante la partida. Su propósito principal es proporcionar un entorno seguro desde el que el jugador pueda interrumpir la acción, consultar controles, modificar el volumen de la música o abandonar la sesión actual sin afectar a la estabilidad de los sistemas en ejecución.

#### Inicialización y Preparación del Entorno

Durante los ciclos `Awake()` y `Start()`, el componente configura los recursos necesarios para el funcionamiento del menú de pausa y sincroniza los valores iniciales de la interfaz con la configuración global de audio.

- **Registro de Instancia Global:** Almacena una referencia estática accesible mediante `Instance`, permitiendo que otros sistemas puedan consultar o invocar funcionalidades relacionadas con la pausa.
    
- **Creación de Canales de Audio Persistentes:** Genera dos fuentes de audio independientes destinadas a la reproducción de efectos de navegación (_Hover_) y confirmación (_Click_).
    
- **Compatibilidad con Estado de Pausa:** Ambos canales son configurados para ignorar la suspensión general del audio producida por `Time.timeScale = 0`, garantizando que la interfaz continúe proporcionando retroalimentación sonora incluso cuando el juego se encuentra detenido.
    
- **Sincronización de Volumen Inicial:** Durante el arranque, el componente recupera el valor actual almacenado en el mezclador de audio (`AudioMixer`) y actualiza la posición del control deslizante (`Slider`) para reflejar la configuración activa del jugador.
    

#### Gestión Centralizada de Retroalimentación Sonora

El componente actúa como un concentrador de eventos acústicos para toda la interfaz de pausa, coordinando la reproducción de sonidos asociados a la navegación y selección de opciones.

**1. Reproducción de Sonidos de Navegación (`PlayHoverSound`)**

Cuando el usuario desplaza el foco entre elementos interactivos, el sistema reproduce el clip solicitado utilizando un canal dedicado exclusivamente a los eventos de navegación.

**2. Reproducción de Sonidos de Confirmación (`UI_PlayClick`)**

Al seleccionar una opción válida, el sistema interrumpe cualquier reproducción secundaria y ejecuta el efecto sonoro global de confirmación configurado para la interfaz.

**3. Corte Temporal de Seguridad**

Con el objetivo de evitar que clips excesivamente largos interfieran con las transiciones de menú, la reproducción de confirmación es detenida automáticamente tras un intervalo de 0,4 segundos mediante una llamada diferida a `StopClickAudio()`.

#### Conmutación del Estado de Pausa

El método `OnTogglePause()` constituye el punto de entrada principal para la activación y desactivación del sistema de pausa. Cuando el jugador ejecuta la acción de entrada asociada, el componente evalúa el estado actual de la partida y selecciona automáticamente la operación correspondiente.

```text
[Input Pause]
       │
       ▼
¿Juego Pausado?
    │      │
   No      Sí
    │      │
    ▼      ▼
[Pausar] [Reanudar]
```

**1. Activación de la Pausa (`Pausar`)**

Cuando la partida se encuentra en ejecución, el sistema realiza las siguientes operaciones:

- Suspende la simulación global mediante `Time.timeScale = 0`.
    
- Marca internamente el estado de pausa como activo.
    
- Habilita la visualización del panel principal de pausa.
    
- Oculta cualquier submenú secundario previamente abierto.
    
- Asigna el foco al primer elemento interactivo configurado para la navegación mediante mando o teclado.
    

**2. Reanudación de la Partida (`Reanudar`)**

Cuando el juego ya se encuentra pausado, el sistema ejecuta el proceso inverso:

- Reproduce el sonido de confirmación.
    
- Restaura la velocidad temporal normal mediante `Time.timeScale = 1`.
    
- Oculta todos los paneles asociados al menú.
    
- Restablece el indicador interno de estado.
    
- Devuelve el control completo al jugador.
    

#### Navegación entre Submenús

Además del panel principal de pausa, el componente administra la transición hacia la sección de controles y configuración.

```text
[Menú de Pausa]
        │
        ▼
[Abrir Controles]
        │
        ▼
[Panel de Controles]
        │
        ▼
[Volver]
        │
        ▼
[Menú de Pausa]
```

**1. Apertura del Panel de Controles (`OpenOptions`)**

Al seleccionar la opción correspondiente, el sistema:

- Reproduce el sonido de confirmación.
    
- Oculta el menú principal de pausa.
    
- Muestra el panel de controles.
    
- Asigna el foco al primer elemento navegable del nuevo menú.
    

**2. Retorno al Menú Principal (`BackToPause`)**

Cuando el jugador abandona la sección de controles:

- Se reproduce el sonido de confirmación.
    
- El panel secundario es ocultado.
    
- El menú principal vuelve a mostrarse.
    
- El foco regresa al botón inicial del menú de pausa.
    

#### Gestión Dinámica del Volumen Musical

El método `SetMusicVolume()` permite modificar en tiempo real el volumen de la música del juego mediante la interacción con un control deslizante.

**1. Conversión de Escala Lineal a Decibelios**

El valor recibido desde la interfaz se transforma a una escala logarítmica compatible con el sistema de mezcla de Unity:

```text
[Valor Slider]
       │
       ▼
[Conversión Logarítmica]
       │
       ▼
[Decibelios]
       │
       ▼
[AudioMixer]
```

**2. Actualización del Mezclador de Audio**

Tras la conversión, el nuevo valor es aplicado al parámetro `"music"` del `AudioMixer`, afectando inmediatamente al volumen de la banda sonora activa.

#### Abandono de la Partida (`Salir`)

Cuando el jugador decide abandonar la sesión actual, el componente ejecuta una secuencia simplificada de salida.

**1. Confirmación Sonora**

Se reproduce el efecto de clic asociado a la selección.

**2. Restauración Temporal**

Antes de abandonar la escena, el sistema restablece `Time.timeScale = 1` para evitar que la siguiente escena herede el estado de pausa.

**3. Carga de Escena**

Finalmente, se invoca `SceneManager.LoadScene(nombreEscena)`, transfiriendo al jugador al destino especificado.

#### Gestión de Navegación mediante Controlador

Como medida de accesibilidad y compatibilidad multiplataforma, el método `FocusButton()` administra la selección activa dentro del `EventSystem`.

Cada vez que se abre un nuevo panel, el sistema elimina cualquier selección previa y asigna el foco al elemento configurado como punto de entrada para la navegación, garantizando una experiencia consistente tanto con teclado como con mando.

#### Garantía de Consistencia Operativa

La combinación de suspensión temporal, control centralizado de paneles, gestión de audio independiente y navegación asistida permite que el menú de pausa funcione como una capa de control desacoplada de la simulación principal. Gracias a esta arquitectura, el jugador puede interrumpir la partida en cualquier momento, modificar configuraciones esenciales o abandonar la sesión de forma segura sin comprometer el estado interno del juego.