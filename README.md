# Geometry Dash en C# — Paradigmas de Programación, Unidad 1

Proyecto de la materia **Paradigmas de Programación** (TecNM Campus Tijuana).
Extendemos el programa orientado a objetos que construimos en clase (cajas y pelotas que rebotan con Raylib) para convertirlo en un juego tipo *Geometry Dash*, y lo comparamos con una versión procedural generada con IA.

**Equipo:** José Franyutti Ricardez · Eduardo Orduño Moreno

**Repositorio:** https://github.com/l25210812-creator/geometry-dash-paradigmas

## Contenido del repositorio

```
.
├── GeometryDashOO/            Nuestra extensión del programa de clase (orientado a objetos)
│   ├── Program.cs
│   └── GeometryDashOO.csproj
├── GeometryDashProcedural/    El mismo juego reescrito en estilo procedural (generado con IA)
│   ├── Program.cs
│   └── GeometryDashProcedural.csproj
├── docs/
│   ├── ProgramaBaseClase.cs   Programa original que armamos en clase
│   └── linea-del-tiempo.html  Línea del tiempo interactiva de los paradigmas (Paradigm Dash)
└── README.md
```

## Cómo se juega

| Tecla | Acción |
|---|---|
| `Espacio` | Saltar |
| `Espacio` (al perder) | Reiniciar la partida |
| `Esc` | Cerrar el juego |

El cubo azul corre sobre el suelo y los obstáculos avanzan hacia él. Cada obstáculo que sale por la izquierda reaparece por la derecha. Si el cubo toca uno, aparece la pantalla de **¡PERDISTE!** con el puntaje.

## Cómo ejecutarlo

Requisitos: [.NET SDK 8](https://dotnet.microsoft.com/download) o superior. La librería **Raylib-cs** se descarga sola desde NuGet.

```bash
# Versión orientada a objetos
cd GeometryDashOO
dotnet run

# Versión procedural
cd GeometryDashProcedural
dotnet run
```

Cada versión es un proyecto separado porque las dos tienen su propio `Main`.

## Qué agregamos al programa de clase

El programa base tenía una clase abstracta `Objeto` con las subclases `Caja`, `Pelota` y `Raqueta`. Sobre esa jerarquía agregamos:

- **`Jugador : Caja`** — sobrescribe `Update()` con gravedad, salto con `Espacio` y detección del suelo.
- **`Obstaculo : Caja`** — sobrescribe `Update()` para avanzar a la izquierda y reaparecer al salir de la pantalla.
- **Estado del juego** — puntaje, estado `muerto`, pantalla de derrota y reinicio.
- **`CrearObstaculos()`** — función que reconstruye la lista de obstáculos al reiniciar.
- Simplificamos `Caja.Update()` y quitamos `Pelota` y `Raqueta`, porque el juego no necesita rebotes.

```
Objeto  (abstracta: Draw, Update, CollisionWith, GetArea)
└── Caja
    ├── Jugador
    └── Obstaculo
```

## Paradigmas que aparecen en el código

| Paradigma | Dónde se ve |
|---|---|
| Orientado a objetos | Clase abstracta `Objeto`, herencia `Caja → Jugador / Obstaculo`, `override` de `Update()`, lista polimórfica `List<Objeto>`, interfaz `IComparable` |
| Imperativo / estructurado | El *game loop*: actualizar, revisar colisiones y dibujar en cada cuadro |
| Dirigido por eventos | `IsKeyDown(Space)` para saltar e `IsKeyPressed(Space)` para reiniciar |
| Procedural | `CrearObstaculos()` y toda la versión de `GeometryDashProcedural` |

## Versión procedural

`GeometryDashProcedural/Program.cs` hace exactamente lo mismo sin clases ni herencia:

- Los datos viven en un `struct Bloque` que solo tiene campos (como un `struct` de C).
- El comportamiento está en funciones independientes: `ActualizarJugador()`, `ActualizarObstaculos()`, `HayColision()`, `DibujarJuego()`.
- El estado es global y el ciclo principal llama a las funciones en orden fijo.

C# obliga a que `Main` viva dentro de una clase, así que `static class Program` solo funciona como contenedor de funciones.

## Línea del tiempo

`docs/linea-del-tiempo.html` es una línea del tiempo jugable: el cubo recorre los hitos de los paradigmas desde 1843 hasta este proyecto. Se abre directamente en el navegador.

## Herramientas

- C# con .NET
- [Raylib-cs](https://github.com/raylib-cs/raylib-cs), versión de C# de [raylib](https://www.raylib.com/)
- Claude (IA) para generar la versión procedural, como pide la actividad
