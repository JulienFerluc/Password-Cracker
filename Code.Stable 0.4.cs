using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

/// <summary>
/// Crackeur de mots de passe Multi-Threadé
/// Julien Ferluc
/// 2016-10-08
/// </summary>

namespace MultiThreadTest
{
    class Program
    {


        // Variables globales
        static long count;
        static string _crack;
        static string password;
        static int debut;
        static int fin;
        static bool fini = false;

        static void Main()
        {
            Console.WriteLine("Initialisation du programme");

            //Setup des variables
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Quel est le mot de passe a deviner?");
            password = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Nombre de lettres minimum?");
            debut = int.Parse(Console.ReadLine());
            Console.Clear();
            Console.WriteLine("Nombre de lettres maximum?");
            fin = int.Parse(Console.ReadLine());
            fin = fin + 1;
            Console.Clear();


            //PID
            int nProcessID = Process.GetCurrentProcess().Id;
            Console.Write(("pid: " + nProcessID).PadRight(11));

            //Date
            Console.WriteLine(DateTime.Now);

            //Initialisation des threads
            Thread worker1 = new Thread(new ThreadStart(Worker1));
            Thread worker2 = new Thread(new ThreadStart(Worker2));
            //Thread worker3 = new Thread(new ThreadStart(Worker3));
            //Thread worker4 = new Thread(new ThreadStart(Worker4));

            worker1.Start();
            worker2.Start();
            //worker3.Start();
            //worker4.Start();

            //Demarage du chronometre
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (true)
            {
                if (fini == true)
                {
                    //Arret des threads
                    worker1.Abort();
                    worker2.Abort();
                    //worker3.Abort();
                    //worker4.Abort();

                    //Arret du timer et conversion du temps en millisecondes
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;

                    //Stats et affichage du mot de passe
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("password: " + _crack);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(count + " tries in " + elapsed + "ms at " + count / elapsed + " tries/ms");
                    break;
                }
            }
            Console.Read();
        }


        public static void Worker1()
        {
            Console.WriteLine("Thread 1 Online");
            string crack = "";
            char rand;
            int num;
            int lenght;
            int i = 0;
            Random random = new Random();
            //Generation random d'un pseudo mot de passe et verification sur la thread 1

            do
            {
                crack = "";
                lenght = 0;
                i = 0;
                lenght = random.Next(debut, fin);
                while (i != lenght)
                {
                    num = random.Next(0, 26);
                    rand = (char)('a' + num);
                    crack = crack.Insert(0, rand.ToString());
                    i++;
                }
                count++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker2()
        {
            Console.WriteLine("Thread 2 Online");
            string crack = "";
            char rand;
            int num;
            int lenght;
            int i = 0;
            Random random = new Random();
            //Generation random d'un pseudo mot de passe et verification sur la thread 1
            do
            {
                crack = "";
                lenght = 0;
                i = 0;
                lenght = random.Next(debut, fin);
                while (i != lenght)
                {
                    num = random.Next(0, 26);
                    rand = (char)('a' + num);
                    crack = crack.Insert(0, rand.ToString());
                    i++;
                }
                count++;
            } while (crack != password);
            count++;
            _crack = crack;
            fini = true;
        }

        /*public static void Worker3()
        {
            Console.WriteLine("Thread 3 Online");
            string crack = "";
            char rand;
            int num;
            int lenght;
            int i = 0;
            Random random = new Random();
            //Generation random d'un pseudo mot de passe et verification sur la thread 1
            do
            {
                crack = "";
                lenght = 0;
                i = 0;
                lenght = random.Next(debut, fin);
                while (i != lenght)
                {
                    num = random.Next(0, 26);
                    rand = (char)('a' + num);
                    crack = crack.Insert(0, rand.ToString());
                    i++;
                }
                count++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker4()
        {
            Console.WriteLine("Thread 4 Online");
            string crack = "";
            char rand;
            int num;
            int lenght;
            int i = 0;
            Random random = new Random();
            //Generation random d'un pseudo mot de passe et verification sur la thread 1
            do
            {
                crack = "";
                lenght = 0;
                i = 0;
                lenght = random.Next(debut, fin);
                while (i != lenght)
                {
                    num = random.Next(0, 26);
                    rand = (char)('a' + num);
                    crack = crack.Insert(0, rand.ToString());
                    i++;
                }
                count++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }*/
    }
}
