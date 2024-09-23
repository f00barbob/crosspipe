using System;
using System.IO.Pipes;
using System.Threading;

namespace crosspipe
{
    internal static class Program
    {
        static NamedPipeClientStream pipe1 = null;
        static NamedPipeClientStream pipe2 = null;
        static bool isExiting;
        static bool debug = false;

        static int Main()
        {
            pipe1 = new NamedPipeClientStream(".", "86Box1", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            pipe2 = new NamedPipeClientStream(".", "86Box2", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            try
            {
                pipe1.Connect(1000);
                pipe2.Connect(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Pipe 1 should be \\\\.\\pipe\\86Box1");
                Console.WriteLine("Pipe 2 should be \\\\.\\pipe\\86Box2");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 1;
            }

            ThreadStart ts1 = new ThreadStart(p1p2);
            ThreadStart ts2 = new ThreadStart(p2p1);

            Thread t1 = new Thread(ts1);
            Thread t2 = new Thread(ts2);
            t1.Start();
            t2.Start();

            Console.WriteLine("Threads started; h for help");

            while (!isExiting)
            {
                Console.WriteLine();
                var k = Console.ReadKey();
                Console.WriteLine();

                switch (k.Key)
                {
                    case ConsoleKey.Q:
                        isExiting = true;
                        break;
                    case ConsoleKey.H:
                        Console.WriteLine("Pipe 1 is \\\\.\\pipe\\86Box1");
                        Console.WriteLine("Pipe 2 is \\\\.\\pipe\\86Box2");
                        Console.WriteLine("h: help");
                        Console.WriteLine("q: quit");
                        Console.WriteLine("d: debug");
                        break;
                    case ConsoleKey.D:
                        debug = !debug;
                        Console.WriteLine("debug=" + debug.ToString());
                        break;
                    default:
                        break;

                }


            }

            t1.Interrupt();
            t2.Interrupt();

            pipe1.Close();
            pipe2.Close();

            pipe1.Dispose();
            pipe2.Dispose();

            return 0;
        }


        static void p1p2()
        {
            int i = 0;

            while (true)
            {
                i++;
                try
                {
                    byte a = (byte)pipe1.ReadByte();

                    if (debug)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1: {0:X02} {1}", (int)a, (char)a);
                        Console.ResetColor();
                    }

                    pipe2.WriteByte(a);
                    pipe2.Flush();
                    pipe2.WaitForPipeDrain();
                }
                catch (Exception ex)
                {
                    Console.Write("p1p2: " + ex.ToString());
                    return;
                }
            }

            if (i == 1000)
            {
                Console.WriteLine(">> 1000b from pipe1");
                i = 0;
            }
        }
            
        static void p2p1()
        {
            int i = 0;
            while (true)
            {
                i++;

                try
                {

                    byte a = (byte)pipe2.ReadByte();
                    // this code has stupid cow powers
                    if (debug)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t2: {0:X02} {1}", (int)a, (char)a);
                        Console.ResetColor();
                    }
                    pipe1.WriteByte(a);
                    pipe1.Flush();
                    pipe1.WaitForPipeDrain();
                }
                catch (Exception ex)
                {
                    Console.Write("p2p1: " + ex.ToString());
                    if (isExiting)
                    {
                        return;
                    }
                }

                if (i == 1000)
                {
                    Console.WriteLine(">> 1000b from pipe2");
                    i = 0;
                }

            }
        }
    }
}

