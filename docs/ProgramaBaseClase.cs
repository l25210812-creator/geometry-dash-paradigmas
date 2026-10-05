using System.ComponentModel;
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
    {     if (o == null) return 1;
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
        if ( o is Pelota)
            return this.CollisionWith((Pelota) o);
        else if (o is Caja)
            return this.CollisionWith((Caja) o);  
        return false;
               
    }
    public  bool CollisionWith(Pelota p)
    {
     return p.CollisionWith(this);  
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
         Raylib.DrawRectangle( (int)  posición.X, (int) posición.Y, ancho, largo, color);
    }
    public override void Update(int screenWidth, int screenHeight)
    {
            posición+=velocidad;

            if (posición.Y + largo >= screenHeight || posición.Y <= 0)
            {
                velocidad.Y = velocidad.Y * -1.0f;
            }
            if  (posición.X <= 0 || posición.X + ancho >= screenWidth)
            {
                velocidad.X = velocidad.X * -1.0f;
            }
    }
}

class Raqueta:Caja
{
    public Raqueta(Vector2 p, Vector2 v, Color c, int a, int l):base(p,v,c, a, l)
    {}

    public override void Update(int screenWidth, int screenHeight)
    {

        if  (posición.X <= 0)
        {

            if  (Raylib.IsKeyDown(KeyboardKey.Right))
               {
                Raylib.DrawText("Derecha", 100,100, 30, Color.Black);
                posición.X+=velocidad.X;
                }
        }
       else  if (posición.X + ancho >= screenWidth)
        {
            if  (Raylib.IsKeyDown(KeyboardKey.Left))
               {
                Raylib.DrawText("Izquierda", 100,100, 30, Color.Black);
                posición.X+=velocidad.X*-1;
              }
        }
        else
        {   
            if  (Raylib.IsKeyDown(KeyboardKey.Left))
               {
                Raylib.DrawText("Izquierda", 100,100, 30, Color.Black);
                posición.X+=velocidad.X*-1;
              }

            if  (Raylib.IsKeyDown(KeyboardKey.Right))
               {
                Raylib.DrawText("Derecha", 100,100, 30, Color.Black);
                posición.X+=velocidad.X;
                }
        }
    }


}
class Pelota:Objeto
{  
    public int radio = 100;
    public override bool CollisionWith(Objeto o)
    {      
        if ( o is Pelota)
            return this.CollisionWith((Pelota) o);
        else if (o is Caja)
            return this.CollisionWith((Caja) o);  
        return false;
               
    }
   
    public bool CollisionWith(Pelota p)
    {
     return Raylib.CheckCollisionCircles(this.posición, this.radio, p.posición, p.radio);  
    }

    public  bool CollisionWith(Caja c)
    {
        return Raylib.CheckCollisionCircleRec(this.posición, this.radio, new Rectangle(c.posición, (float) c.ancho, (float) c.largo));
    }

    public override int GetArea()
    {
        return (int) (Math.PI * radio * radio);
    }
    public Pelota(Vector2 p, Vector2 v, Color c, int r):base(p,v,c)
    {  
        // corrección de posición (para que no se quede atrapado)
        p.X = p.X < r ?  p.X + r + 2 : p.X;
        p.Y = p.Y < r ?  p.Y + r + 2 : p.Y;
       
        posición=p;
        velocidad=v;
        radio = r;
        color = c;
    }
    public override void Draw()
    {
         Raylib.DrawCircle( (int)  posición.X, (int) posición.Y, radio, color);
    }
    public override void Update(int screenWidth, int screenHeight)
    {
            posición+=velocidad;

            if (posición.Y + radio >= screenHeight || posición.Y - radio <= 0)
            {
                velocidad.Y = velocidad.Y * -1.0f;
            }
            // El else estaba mal.
            if  (posición.X - radio <= 0 || posición.X + radio >= screenWidth)
            {
                velocidad.X = velocidad.X * -1.0f;
            }
    }
}

internal static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows
    // See https://github.com/raylib-cs/raylib-cs/issues/301
    [System.STAThread]
    public static void Main()
    {
        // Inicializa
        //----------------------------------------------------------------
        const int screenWidth = 1200;
        const int screenHeight = 850;



        Raylib.InitWindow(screenWidth, screenHeight, "Hello World");
        Raylib.SetTargetFPS(60);

        //------------------------------------------------------------------
        Raqueta r = new Raqueta(new Vector2(100, screenHeight-100), new Vector2(4.0f,0.0f), Color.Black, 100, 20);  
        List<Objeto> objetos = new List<Objeto>();
        objetos.Add(new Pelota(new Vector2(40.0f,40.0f), new Vector2(1.0f,1.0f), Color.Brown, 60));
        objetos.Add(new Caja(new Vector2(200.0f,200.0f), new Vector2(-1.0f,-1.0f), Color.Pink, 80,80));
        objetos.Add(new Pelota(new Vector2(250.0f,100.0f), new Vector2(2.0f,-2.0f), Color.DarkGray, 10));
        objetos.Add(new Caja(new Vector2(200.0f,200.0f), new Vector2(-2.0f,1.0f), Color.Purple, 40,80));
        objetos.Sort(); // Area

        // Game loop
        while (!Raylib.WindowShouldClose())
        {
            // Actualiza el estado
            //---------------------------------------------------------------
            // TODO: Actualiza el estado aquí
            //----------------------------------------------------------------
           
            foreach (var o in objetos)
            {
                o.color = o.colorOriginal; 
                o.Update(screenWidth, screenHeight);
            }
            
            for (int i = 0;  i < objetos.Count; i++)
               for(int j = i+1; j < objetos.Count; j++)
            {
                if (objetos[i].CollisionWith(objetos[j]))
                    {
                        objetos[i].color = Color.Red;
                        objetos[j].color = Color.Red;
                    }
            }
            r.Update(screenWidth,screenHeight); 
            
            //
            // Draw
            //----------------------------------------------------------------
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);
              
            r.Draw();            
            foreach (var p in objetos)
                    p.Draw();
           
            Raylib.EndDrawing();
            //------------------------------------------------------------------
        }

        // Cierra
        //----------------------------------------------------------------------
        Raylib.CloseWindow();
    }
}