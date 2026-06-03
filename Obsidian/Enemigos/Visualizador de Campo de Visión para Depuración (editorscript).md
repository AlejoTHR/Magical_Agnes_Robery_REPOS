Este componente extiende las herramientas del editor de Unity para proporcionar una representación visual del sistema de detección utilizado por las entidades enemigas. Su propósito principal consiste en facilitar las tareas de diseño, ajuste y depuración del comportamiento de percepción artificial, dibujando directamente sobre la vista de escena los límites del campo de visión y los objetivos actualmente detectados por cada enemigo.

A diferencia de los componentes de ejecución habituales, este sistema opera exclusivamente dentro del entorno de desarrollo y no forma parte de la lógica utilizada durante la partida final.

#### Integración con el Editor de Unity

El componente utiliza el atributo:

```csharp
[CustomEditor(typeof(EnemyScript))]
```

para asociarse automáticamente a todas las instancias de `EnemyScript` presentes en la escena.

Gracias a esta vinculación, Unity ejecuta el método `OnSceneGUI()` cada vez que un enemigo es seleccionado desde el editor, permitiendo generar información visual adicional directamente sobre la ventana _Scene View_.

```text
[Seleccionar EnemyScript]
            │
            ▼
      [OnSceneGUI]
            │
            ▼
[Generar Visualización]
```

#### Representación del Campo de Visión

Durante cada actualización de la vista de escena, el sistema obtiene una referencia al enemigo seleccionado y utiliza la API `Handles` para construir una representación gráfica de su área de detección.

**1. Visualización del Radio de Detección**

El componente dibuja un arco circular centrado en la posición del enemigo utilizando el valor almacenado en `viewRadius`.

```text
            _______
        .-´         `-.
      /                 \
     |       Enemy       |
      \                 /
        `-._______.-´

      Radio = viewRadius
```

Esta representación permite verificar visualmente el alcance máximo de detección configurado para cada entidad.

**2. Representación del Ángulo de Visión**

A partir del valor definido en `viewAngle`, el sistema calcula los límites izquierdo y derecho del cono visual mediante llamadas al método `DirFromAngle()`.

Posteriormente, se dibujan dos líneas que delimitan el área efectiva de visión.

```text
             /
            /
           /
      Enemy
           \
            \
             \
```

Esta información resulta especialmente útil para ajustar patrullas, comportamientos de vigilancia y zonas de detección.

#### Visualización de Objetivos Detectados

Además de representar el área potencial de observación, el sistema muestra en tiempo real qué objetivos están siendo percibidos actualmente por la inteligencia artificial.

```text
[Enemy]
    │
    ├────────► Target A
    │
    ├────────► Target B
    │
    └────────► Target C
```

**1. Consulta de Objetivos Visibles**

El componente recorre la colección `visibleTargets`, mantenida por el sistema de percepción del enemigo.

**2. Generación de Conexiones Visuales**

Para cada objetivo registrado, se dibuja una línea roja que conecta la posición del enemigo con la posición del objetivo detectado.

Estas conexiones proporcionan una representación inmediata de los resultados producidos por el algoritmo de detección, facilitando la identificación de errores de configuración o comportamientos inesperados.

#### Utilidad Durante el Desarrollo

Este sistema está diseñado exclusivamente como herramienta de apoyo para diseñadores y programadores durante la construcción de niveles y el ajuste de inteligencia artificial.

Entre sus principales aplicaciones se encuentran:

- Verificación de radios de detección.
    
- Ajuste de ángulos de visión.
    
- Validación de obstáculos y líneas de visión.
    
- Depuración de comportamientos de persecución.
    
- Comprobación de objetivos detectados en tiempo real.
    

Al ejecutarse únicamente dentro del editor, el componente no introduce costes de rendimiento ni lógica adicional en las versiones finales del juego.

#### Garantía de Depuración Visual

La combinación de representación geométrica del campo de visión y visualización directa de los objetivos detectados convierte este componente en una herramienta de diagnóstico para el sistema de percepción enemigo. 