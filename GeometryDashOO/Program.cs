using System.Numerics;
using Raylib_cs;

namespace HelloWorld;

abstract class Objeto: IComparable<Objeto>
{
 public Objeto(Vector2 p, Vector2 v, Color c)
    {
        posición = p;
        velocidad = v;
        color = c;
        colorOriginal = c;
    }

public virtual int CompareTo(Objeto? o)
    {     if (o == null) return 1;
          return GetArea().CompareTo(o.GetArea());
    }

 public abstract int GetArea();  
 public abstract void Draw();  
 public abstract void Update(int screenWidth, int screenHeight);
 public abstract bool CollisionWith(Objeto o);
 public Vector2 posición;
 public Vector2 velocidad;    
 public Color color;
 public Color colorOriginal;
}

class Caja : Objeto
{
    public int ancho;
    public int largo;

    public Caja(Vector2 p, Vector2 v, Color c, int a, int l):base(p,v,c)
    {
        ancho=a;
        largo=l;
    }

    public override bool CollisionWith(Objeto o)
    {      
        if ( o is Caja)
            return this.CollisionWith((Caja) o);  
        return false;
    }

    public bool CollisionWith(Caja c)
    {
        return Raylib.CheckCollisionRecs(new Rectangle(this.posición, (float) this.ancho, (float) this.largo),
                                         new Rectangle(c.posición, (float) c.ancho, (float) c.largo));
    }

    public override int GetArea()
    {
        return largo * ancho;
    }

    public override void Draw()
    {
         Raylib.DrawRectangle( (int)  posición.X, (int) posición.Y, ancho, largo, color);
    }

    public override void Update(int screenWidth, int screenHeight)
    {
            posición+=velocidad;
    }
}

//cubo que salta con espacio
class Jugador : Caja
{
    public bool enSuelo = true;

    public Jugador(Vector2 p, Color c) : base(p, new Vector2(0f, 0f), c, 50, 50)
    {}

    public override void Update(int screenWidth, int screenHeight)
    {
        int suelo = screenHeight - 100;

        // Salto
        if (enSuelo && Raylib.IsKeyDown(KeyboardKey.Space))
        {
            velocidad.Y = -15f;
            enSuelo = false;
        }

        // Gravedad
        velocidad.Y += 0.8f;
        posición.Y += velocidad.Y;

        // Suelo
        if (posición.Y + largo >= suelo)
        {
            posición.Y = suelo - largo;
            velocidad.Y = 0f;
            enSuelo = true;
        }
    }
}

// obstáculos
class Obstaculo : Caja
{
    public Obstaculo(Vector2 p, Color c, int a, int l) : base(p, new Vector2(-6.0f, 0.0f), c, a, l)
    {}

    public override void Update(int screenWidth, int screenHeight)
    {
        posición += velocidad;

        if (posición.X + ancho < 0)
        {
            posición.X += 1800;
        }
    }
}

internal static class Program
{
    static void CrearObstaculos(List<Objeto> objetos, int sueloY)
    {
        objetos.Clear();
        objetos.Add(new Obstaculo(new Vector2(800, sueloY - 50), Color.DarkGray, 50, 50));
        objetos.Add(new Obstaculo(new Vector2(1300, sueloY - 80), Color.Purple, 50, 80));
        objetos.Add(new Obstaculo(new Vector2(1700, sueloY - 50), Color.Brown, 50, 50));
    }

    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    [System.STAThread]
    public static void Main()
    {
        // Inicializa
        //----------------------------------------------------------------
        const int screenWidth = 1200;
        const int screenHeight = 850;
        int sueloY = screenHeight - 100;

        Raylib.InitWindow(screenWidth, screenHeight, "Geometry Dash");
        Raylib.SetTargetFPS(60);

        //------------------------------------------------------------------
        Jugador jugador = new Jugador(new Vector2(200, sueloY - 50), Color.Blue);
        List<Objeto> objetos = new List<Objeto>();
        CrearObstaculos(objetos, sueloY);
        int puntos = 0;
        bool muerto = false;

        // Game loop
        while (!Raylib.WindowShouldClose())
        {
            // se actualiza el estado
            //---------------------------------------------------------------
            if (!muerto)
            {
                foreach (var o in objetos)
                {
                    o.Update(screenWidth, screenHeight);
                }
                jugador.Update(screenWidth, screenHeight);
                puntos++;

                // Si toca un obstáculo, termina la partida
                foreach (var o in objetos)
                {
                    if (jugador.CollisionWith(o))
                    {
                        muerto = true;
                        jugador.color = Color.Red;
                        break;
                    }
                }
            }
            else
            {
                // Pantalla de "perdiste"
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    jugador.posición = new Vector2(200, sueloY - 50);
                    jugador.velocidad = new Vector2(0f, 0f);
                    jugador.color = jugador.colorOriginal;
                    CrearObstaculos(objetos, sueloY);
                    puntos = 0;
                    muerto = false;
                }
            }

            //
            // Draw
            //----------------------------------------------------------------
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);
            Raylib.DrawRectangle(0, sueloY, screenWidth, 100, Color.Black);
            Raylib.DrawText($"Puntos: {puntos / 10}", 20, 20, 30, Color.Blue);
            jugador.Draw();
            foreach (var o in objetos)
                    o.Draw();

            // Cartel de "perdiste"
            if (muerto)
            {
                string t1 = "¡PERDISTE!";
                string t2 = $"Puntaje: {puntos / 10}";
                string t3 = "Presiona ESPACIO para continuar";

                Raylib.DrawRectangle(300, 250, 600, 300, Color.Black);
                Raylib.DrawText(t1, (screenWidth - Raylib.MeasureText(t1, 60)) / 2, 290, 60, Color.Red);
                Raylib.DrawText(t2, (screenWidth - Raylib.MeasureText(t2, 40)) / 2, 380, 40, Color.White);
                Raylib.DrawText(t3, (screenWidth - Raylib.MeasureText(t3, 25)) / 2, 470, 25, Color.LightGray);
            }

            Raylib.EndDrawing();
            //------------------------------------------------------------------
        }

        // Cierra
        //----------------------------------------------------------------------
        Raylib.CloseWindow();
    }
}