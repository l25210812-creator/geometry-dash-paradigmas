// =====================================================================
//  Geometry Dash - VERSIÓN PROCEDURAL
//  Generada con IA (Claude) a partir de la versión orientada a objetos.
//
//  Diferencias clave respecto a la versión OO:
//   - No hay clases con herencia, métodos virtuales ni polimorfismo.
//   - Los datos viven en un "struct" plano (solo campos, como en C).
//   - El comportamiento está en funciones estáticas independientes
//     que reciben o modifican los datos.
//   - El estado del juego es global (variables estáticas).
//  C# obliga a que Main viva dentro de una clase, por eso existe
//  "static class Program", pero se usa solo como contenedor de funciones.
// =====================================================================

using Raylib_cs;

namespace GeometryDashProcedural;

// Estructura de datos "pura": no tiene métodos, solo información.
struct Bloque
{
    public float x;
    public float y;
    public float vx;
    public float vy;
    public int ancho;
    public int alto;
    public Color color;
}

internal static class Program
{
    // ---------------- Constantes del juego ----------------
    const int ANCHO_PANTALLA = 1200;
    const int ALTO_PANTALLA = 850;
    const int SUELO_Y = ALTO_PANTALLA - 100;
    const float GRAVEDAD = 0.8f;
    const float FUERZA_SALTO = -15f;
    const float VELOCIDAD_OBSTACULO = -6f;
    const int DISTANCIA_REAPARICION = 1800;
    const int NUM_OBSTACULOS = 3;

    // ---------------- Estado global del juego ----------------
    static Bloque jugador;
    static Color colorJugador = Color.Blue;
    static bool enSuelo;
    static Bloque[] obstaculos = new Bloque[NUM_OBSTACULOS];
    static int puntos;
    static bool muerto;

    // ---------------- Funciones de creación ----------------
    static Bloque CrearBloque(float x, float y, float vx, int ancho, int alto, Color color)
    {
        Bloque b;
        b.x = x;
        b.y = y;
        b.vx = vx;
        b.vy = 0f;
        b.ancho = ancho;
        b.alto = alto;
        b.color = color;
        return b;
    }

    static void ReiniciarJuego()
    {
        jugador = CrearBloque(200, SUELO_Y - 50, 0f, 50, 50, colorJugador);
        enSuelo = true;

        obstaculos[0] = CrearBloque(800,  SUELO_Y - 50, VELOCIDAD_OBSTACULO, 50, 50, Color.DarkGray);
        obstaculos[1] = CrearBloque(1300, SUELO_Y - 80, VELOCIDAD_OBSTACULO, 50, 80, Color.Purple);
        obstaculos[2] = CrearBloque(1700, SUELO_Y - 50, VELOCIDAD_OBSTACULO, 50, 50, Color.Brown);

        puntos = 0;
        muerto = false;
    }

    // ---------------- Funciones de actualización ----------------
    static void ActualizarJugador()
    {
        // Salto
        if (enSuelo && Raylib.IsKeyDown(KeyboardKey.Space))
        {
            jugador.vy = FUERZA_SALTO;
            enSuelo = false;
        }

        // Gravedad
        jugador.vy += GRAVEDAD;
        jugador.y += jugador.vy;

        // Suelo
        if (jugador.y + jugador.alto >= SUELO_Y)
        {
            jugador.y = SUELO_Y - jugador.alto;
            jugador.vy = 0f;
            enSuelo = true;
        }
    }

    static void ActualizarObstaculos()
    {
        for (int i = 0; i < NUM_OBSTACULOS; i++)
        {
            obstaculos[i].x += obstaculos[i].vx;

            // Si sale por la izquierda, reaparece por la derecha
            if (obstaculos[i].x + obstaculos[i].ancho < 0)
            {
                obstaculos[i].x += DISTANCIA_REAPARICION;
            }
        }
    }

    // Colisión rectángulo-rectángulo (AABB) escrita a mano
    static bool HayColision(Bloque a, Bloque b)
    {
        return a.x < b.x + b.ancho &&
               a.x + a.ancho > b.x &&
               a.y < b.y + b.alto &&
               a.y + a.alto > b.y;
    }

    static void RevisarColisiones()
    {
        for (int i = 0; i < NUM_OBSTACULOS; i++)
        {
            if (HayColision(jugador, obstaculos[i]))
            {
                muerto = true;
                jugador.color = Color.Red;
                return;
            }
        }
    }

    static void ActualizarJuego()
    {
        if (!muerto)
        {
            ActualizarObstaculos();
            ActualizarJugador();
            puntos++;
            RevisarColisiones();
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            ReiniciarJuego();
        }
    }

    // ---------------- Funciones de dibujo ----------------
    static void DibujarBloque(Bloque b)
    {
        Raylib.DrawRectangle((int)b.x, (int)b.y, b.ancho, b.alto, b.color);
    }

    static void DibujarTextoCentrado(string texto, int y, int tamano, Color color)
    {
        int x = (ANCHO_PANTALLA - Raylib.MeasureText(texto, tamano)) / 2;
        Raylib.DrawText(texto, x, y, tamano, color);
    }

    static void DibujarPantallaPerdiste()
    {
        Raylib.DrawRectangle(300, 250, 600, 300, Color.Black);
        DibujarTextoCentrado("¡PERDISTE!", 290, 60, Color.Red);
        DibujarTextoCentrado($"Puntaje: {puntos / 10}", 380, 40, Color.White);
        DibujarTextoCentrado("Presiona ESPACIO para continuar", 470, 25, Color.LightGray);
    }

    static void DibujarJuego()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);

        // Suelo y puntaje
        Raylib.DrawRectangle(0, SUELO_Y, ANCHO_PANTALLA, 100, Color.Black);
        Raylib.DrawText($"Puntos: {puntos / 10}", 20, 20, 30, Color.Blue);

        DibujarBloque(jugador);
        for (int i = 0; i < NUM_OBSTACULOS; i++)
        {
            DibujarBloque(obstaculos[i]);
        }

        if (muerto)
        {
            DibujarPantallaPerdiste();
        }

        Raylib.EndDrawing();
    }

    // ---------------- Programa principal ----------------
    [System.STAThread]
    public static void Main()
    {
        Raylib.InitWindow(ANCHO_PANTALLA, ALTO_PANTALLA, "Geometry Dash (procedural)");
        Raylib.SetTargetFPS(60);

        ReiniciarJuego();

        while (!Raylib.WindowShouldClose())
        {
            ActualizarJuego();
            DibujarJuego();
        }

        Raylib.CloseWindow();
    }
}
