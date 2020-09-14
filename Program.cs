using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace smartsnake
{
    class Program
    {
        /** 
              * <summary>Structura Position.</summary>
              */
        public struct Position
        {
            public int X, Y;
            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        /** 
              * <summary>Structura Manzanita.</summary>
              */
        public struct Manzanita
        {
            public int X, Y;
            public Manzanita(int x, int y)
            {
                X = x;
                Y = y;
            }

            /** 
              * <summary>Generar aleatoreamente nueva manzanita.</summary>
              */
            public void Respawn()
            {
                Random rnd = new Random();
                X = rnd.Next(2, 148);
                Y = rnd.Next(0, 28);
            }

            /** 
              * <summary>Dibujar Manzanita en consola.</summary>
              */
            public void Draw()
            {
                Console.SetCursorPosition(X, Y);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("@");
            }
        }

        /** 
              * <summary>Structura Snake.</summary>
              */
        public struct Snake
        {
            public int X, Y, dX, dY, bodyLen, steps, appleX, appleY;
            //Posiciones del cuerpo del snake
            public List<Position> bodyPositions;
            //Posible camino inteligente que tomara el snake
            public List<Position> smartPath;
            //variables para cambiar funcionalidad del snake
            //isSmart define si nuestro SmartSnake se movera mediante un algoritmo o si lo moveremos nosotros mismos
            //Si isSmart==true byStep define si el algoritmo de IA se ejecutara en cada paso o hallara un camino total
            public bool bodyFull, grow, isSmart, newGoal, byStep;

            public Snake(int x, int y)
            {
                X = x;
                Y = y;
                dX = 1;
                dY = 0;
                bodyLen = 1;
                bodyFull = false;
                grow = false;
                bodyPositions = new List<Position>();
                smartPath = new List<Position>();
                appleX = 0;
                appleY = 0;
                newGoal = true;
                isSmart = false;
                byStep = false;
                steps = 0;
            }

            /** 
              * <summary>Funcion para redireccionar el snake con las variables diferenciales dX dY.</summary>
              * <param name="myKey">Tecla presionada.</param>
              * <returns>Define dX y dY con los valores necesarios para mover nuestro SmartSnake (0,1,-1).</returns> 
              */
            public void redirect(ConsoleKey myKey)
            {

                switch (myKey)
                {
                    case ConsoleKey.W:
                        if (dY != 1)
                        {
                            dY = -1;
                            dX = 0;
                        }
                        break;
                    case ConsoleKey.S:
                        if (dY != -1)
                        {
                            dY = 1;
                            dX = 0;
                        }
                        break;
                    case ConsoleKey.A:
                        if (dX != 1)
                        {
                            dY = 0;
                            dX = -1;
                        }
                        break;
                    case ConsoleKey.D:
                        if (dX != -1)
                        {
                            dY = 0;
                            dX = 1;
                        }
                        break;
                }

            }

            /** 
              * <summary>Calculo de la heuristica para encontrar la distancia a la que se encuentran 2 puntos.</summary>
              * <param name="x1">Posición X a evaluar.</param>
              * <param name="y1">Posición Y a evaluar.</param>
              * <param name="x2">Posición X de la meta (manzanita).</param>
              * <param name="y2">Posición Y de la meta (manzanita).</param>
              * <returns>Distancia entre el punto y la meta (manzanita).</returns> 
              */
            public int distance(int x1, int y1, int x2, int y2)
            {
                return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            }

            /** 
              * <summary>Verifica si la posición ingresada choca con el cuerpo del SmartSnake.</summary>
              * <param name="p">Posición a evaluar.</param>
              * <returns>True en caso se choque a su propio cuerpo, False caso contrario.</returns> 
              */
            public bool autoKill(Position p)
            {
                for (int i = 0; i < bodyPositions.Count; i++)
                {
                    if (p.X == bodyPositions[i].X && p.Y == bodyPositions[i].Y)
                    {
                        return true;
                    }
                }

                return false;
            }

            /** 
              * <summary>Verifica si la posición ingresada coincide con la meta (manzanita).</summary>
              * <param name="x">Posición X a evaluar.</param>
              * <param name="y">Posición Y a evaluar.</param>
              * <returns>True en caso se llegue a la meta con la posicion, False caso contrario.</returns> 
              */
            public bool moveToApple(int x, int y)
            {
                if (x == appleX && y == appleY)
                {
                    return true;
                }
                return false;
            }

            /**
            *   <sumary>Busqueda de un CAMINO optimo con el algoritmo de HillClimbing</sumary>
            */
            public void findSmartPath()
            {
                //Solo buscara un nuevo camino inteligente si existe un nuevo objetivo
                if (newGoal)
                {
                    //Definicion de la meta
                    Position goal = new Position(appleX, appleY);
                    //Posible sucesor para el camino inteligente
                    Position sucesor = new Position(X, Y);
                    //Verificacion si se llego a la meta
                    bool success = false;
                    int tempX = X, tempY = Y;
                    //Camino inteligente que tomara nuestro SmartSnake
                    smartPath = new List<Position>();
                    steps = 0;

                    while (!success)
                    {
                        //Si llega a la meta lo agregamos al camino
                        if (moveToApple(sucesor.X + 1, sucesor.Y))
                        {
                            sucesor.X += 1;
                            smartPath.Add(sucesor);
                            success = true;
                            break;
                        }
                        else if (moveToApple(sucesor.X, sucesor.Y + 1))
                        {
                            sucesor.Y += 1;
                            smartPath.Add(sucesor);
                            success = true;
                            break;
                        }
                        else if (moveToApple(sucesor.X - 1, sucesor.Y))
                        {
                            sucesor.X -= 1;
                            smartPath.Add(sucesor);
                            success = true;
                            break;
                        }
                        else if (moveToApple(sucesor.X, sucesor.Y - 1))
                        {
                            sucesor.Y -= 1;
                            smartPath.Add(sucesor);
                            success = true;
                            break;
                        }

                        //Definicion de movimientos validos
                        bool top = !autoKill(new Position(sucesor.X, sucesor.Y - 1));
                        bool bot = !autoKill(new Position(sucesor.X, sucesor.Y + 1));
                        bool left = !autoKill(new Position(sucesor.X - 1, sucesor.Y));
                        bool right = !autoKill(new Position(sucesor.X + 1, sucesor.Y));

                        //Definir una distancia minima
                        int minDistance = 1000;

                        // Verificación si se mueve hacia arriba
                        // Debido a que HillClimbing es un algoritmo Voraz
                        // Siempre considera la menor distancia local
                        if (top && distance(sucesor.X, sucesor.Y - 1, goal.X, goal.Y) <= minDistance)
                        {
                            tempX = sucesor.X;
                            tempY = sucesor.Y - 1;
                            minDistance = distance(sucesor.X, sucesor.Y - 1, goal.X, goal.Y);
                        }

                        // Verificación si se mueve hacia abajo
                        if (bot && distance(sucesor.X, sucesor.Y + 1, goal.X, goal.Y) <= minDistance)
                        {
                            tempX = sucesor.X;
                            tempY = sucesor.Y + 1;
                            minDistance = distance(sucesor.X, sucesor.Y + 1, goal.X, goal.Y);
                        }

                        // Verificación si se mueve hacia la izquierda
                        if (left && distance(sucesor.X - 1, sucesor.Y, goal.X, goal.Y) <= minDistance)
                        {
                            tempX = sucesor.X - 1;
                            tempY = sucesor.Y;
                            minDistance = distance(sucesor.X - 1, sucesor.Y, goal.X, goal.Y);
                        }

                        // Verificación si se mueve hacia la derecha
                        if (right && distance(sucesor.X + 1, sucesor.Y - 1, goal.X, goal.Y) <= minDistance)
                        {
                            tempX = sucesor.X + 1;
                            tempY = sucesor.Y;
                            minDistance = distance(sucesor.X + 1, sucesor.Y, goal.X, goal.Y);
                        }

                        //Nuevo sucesor
                        sucesor.X = tempX;
                        sucesor.Y = tempY;

                        //Añadir sucesor al camino inteligente
                        smartPath.Add(sucesor);
                    }
                    //Una vez definido el camino, ya no quedaria una nueva meta (restaria mover nuestro SmartSnake por el camino)
                    newGoal = false;
                }
            }

            /**
            *   <sumary>Busqueda de un PASO optimo con el algoritmo de HillClimbing</sumary>
            */
            public void smartStep()
            {

                int tempX = X;
                int tempY = Y;

                //Definicion de movimientos validos
                bool top = !autoKill(new Position(X, Y - 1));
                bool bot = !autoKill(new Position(X, Y + 1));
                bool left = !autoKill(new Position(X - 1, Y));
                bool right = !autoKill(new Position(X + 1, Y));

                //Definir una distancia minima
                int minDistance = 1000;

                // Verificación si se mueve hacia arriba
                // Debido a que HillClimbing es un algoritmo Voraz
                // Siempre considera la menor distancia local
                // POR PASO
                if (top && distance(X, Y - 1, appleX, appleY) <= minDistance)
                {
                    tempX = X;
                    tempY = Y - 1;
                    minDistance = distance(X, Y - 1, appleX, appleY);
                }

                if (bot && distance(X, Y + 1, appleX, appleY) <= minDistance)
                {
                    tempX = X;
                    tempY = Y + 1;
                    minDistance = distance(X, Y + 1, appleX, appleY);
                }

                if (left && distance(X - 1, Y, appleX, appleY) <= minDistance)
                {
                    tempX = X - 1;
                    tempY = Y;
                    minDistance = distance(X - 1, Y, appleX, appleY);
                }

                if (right && distance(X + 1, Y - 1, appleX, appleY) <= minDistance)
                {
                    tempX = X + 1;
                    tempY = Y;
                    minDistance = distance(X + 1, Y, appleX, appleY);
                }

                // SI NUESTRO SMARTSNAKE SE ENCUENTRA SIN SALIDA (POSIBLE CUADRADO)
                // SE ACABA EL JUEGO Y MUESTRA PUNTAJE FINAL
                if (!top && !bot && !left && !right)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.SetCursorPosition(50, 15);
                    Console.Write("Puntaje Final: " + bodyLen);
                    Console.Title = ":( ";
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                // Ya que este algoritmo es por paso, se setean las variables X y Y a las nuevas posiciones
                X = tempX;
                Y = tempY;

            }

            /**
            *   <sumary>Mover (definir nuevos valores para X y Y) nuestro SmartSnake</sumary>
            */
            public void moveSnake()
            {
                //En el caso sea inteligente pero NO POR PASO
                //Se seguira el paso a paso del camino inteligente
                if (isSmart && !byStep)
                {

                    X = smartPath[steps].X;
                    Y = smartPath[steps].Y;

                    steps++;
                }
                //En el caso sea inteligente pero POR PASO
                //Se aplicara la funcion smartStep, la cual setea X y Y a una nueva posicion Optima
                else if (isSmart && byStep)
                {

                    if (!grow)
                    {
                        smartStep();
                    }
                }
                //En el caso que no sea inteligente
                //X y Y se regiran bajo los diferenciales dX y Dy definido por las teclas que ingrese el usuario
                else
                {
                    X += dX;
                    Y += dY;
                }

                //Definicion de limites por el mapa
                if (X < 0) X = 149;
                if (Y < 0) Y = 29;
                if (X > 149) X = 0;
                if (Y > 29) Y = 0;

                //Realizar el dibujo de nuestro SmartSnake en la consola
                drawSnake();
            }

            /**
            *   <sumary>Aumentar el tamaño del cuerpo de nuestro SmartSnake</sumary>
            */
            public void growUp()
            {
                bodyLen++;
                grow = true;
            }

            /**
            *   <sumary>Dibujar nuestro SmartSnake en consola</sumary>
            */
            public void drawSnake()
            {
                if (!bodyFull)
                {
                    for (int i = 0; i < bodyLen; i++)
                    {
                        Console.SetCursorPosition(X, Y);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("*");
                        bodyPositions.Add(new Position(X, Y));

                    }
                    bodyFull = true;
                }
                else
                {
                    Console.SetCursorPosition(bodyPositions[0].X, bodyPositions[0].Y);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                    bodyPositions.RemoveAt(0);
                    Console.SetCursorPosition(X, Y);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("*");
                    bodyPositions.Add(new Position(X, Y));
                    if (grow)
                    {
                        X += dX;
                        Y += dY;
                        if (X < 1) X = 149;
                        if (Y < 1) Y = 29;
                        if (X > 149) X = 0;
                        if (Y > 29) Y = 0;
                        Console.SetCursorPosition(X, Y);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("*");
                        bodyPositions.Add(new Position(X, Y));
                        grow = false;
                    }
                }

            }
        }

        //Funcion iniciadora de la aplicacion por consola
        static void Main(string[] args)
        {

            Random rnd = new Random();
            //Generamos la primera manzanita en la consola
            Manzanita myApple = new Manzanita(20, 20);
            //Seteamos las medidas de la consola (DINAMICO EN UN FUTURO)
            Console.SetWindowSize(150, 30);
            //Creamos nuestro SmartSnake
            Snake mySnake = new Snake(10, 10);
            //Definimos si nuestro SmartSnake es inteligente
            mySnake.isSmart = true;
            //Si seguira un algoritmo paso a paso
            mySnake.byStep = true;
            //Le asignamos una meta
            mySnake.appleX = myApple.X;
            mySnake.appleY = myApple.Y;
            //Corremos en otro hilo la verificacion de presion de tecla por usuario
            var keyPressEvent = Task.Run(() => mySnake.redirect(Console.ReadKey(true).Key));
            ConsoleKey mykey = new ConsoleKey();
            var keyHold = Task.Run(() => mykey = Console.ReadKey(true).Key);
            //Ejecutamos el bucle infinito para correr nuestro juego autonomo
            while (true)
            {
                Console.SetCursorPosition(1, 0);
                //Contabilizamos el marcador en el titulo de la consola
                Console.Title = "WANNA BE SMART SNAKE ------ MANZANITAS COMIDAS -> " + (mySnake.bodyLen - 1);
                //Damos un tiempo de "pausa" a la ejecucion para observar el comportamiento
                Thread.Sleep(20);
                //Si es snake es inteligente y no es POR PASO necesitamos ejecutar la funcion que nos calcula el camino optimo
                /* mySnake.findSmartPath(); */
                //Movemos nuestro SmartSnake constantemente segun sus diferenciales o metricas de IA
                mySnake.moveSnake();
                //Dibujarmos constantemente nuestro SmartSnake
                myApple.Draw();
                //Tratamiento en caso nuestro SmartSnake "coma" la manzanita
                if (mySnake.X == myApple.X && mySnake.Y == myApple.Y)
                {
                    Console.Beep(200, 200);
                    mySnake.growUp();
                    myApple.Respawn();

                    mySnake.newGoal = true;
                    mySnake.steps = 0;
                    myApple.Respawn();
                    mySnake.appleX = myApple.X;
                    mySnake.appleY = myApple.Y;
                }
                if (keyPressEvent.IsCompleted) keyPressEvent = Task.Run(() => mySnake.redirect(Console.ReadKey(true).Key));

            }
        }



    }
}
