using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;



/// <summary>
/// Crackeur de mots de passe Multi-Threadé
/// Julien Ferluc
/// 2016-10-08
/// </summary>

namespace MultiThreadTest
{
    class Program
    {
        //Initializing some global variables (do not put variables that need to be accessed very often by multiple threads here because it will
        //probably slow down the program and render it somewhat unstable)
        //Especially over 2 threads
        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        static long count1, count2, count3, count4, count5, count6, count7, count8;
        static string _crack;
        static string _password;
        static int _debut;
        static int _fin;
        static bool fini = false;
        static long countAll;
        static Process ExternalProcess = new Process();

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Program initialized\nReference: http://web.cs.du.edu/~mitchell/forensics/information/pass_crack.html \n");

            Console.Clear();
            //Selecting the method of cracking the password
            Console.WriteLine("What method would you like to use?");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1 = Random: Proceed by randomly generating passwords\n**Really long and taxing on CPU, needs extra parameters (not recommended for long passwords)\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("2 = Brute-Force: Use a file containing passwords\n**Fast! Taxing on RAM (may never find password depending on file size)\n*Warning* Files over more or less 200Mb may crash this software\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("3 = Random Brute-Force to local program: Try passwords for local programs\n**Output to programs\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("4 = Dictionnary to local program: Try passwords from dictionnary to program\n**Takes a lot of ram for huge dictionnaries**");

            Console.ForegroundColor = ConsoleColor.White;
            string method = Console.ReadLine();

            Console.Clear();

            var stopwatch = new Stopwatch();

            switch (method)
            {
                case "1":
                    Console.WriteLine("What is the password to guess?");
                    _password = Console.ReadLine();
                    string password = _password;
                    int passLength = _password.Length;

                    //min
                    Console.WriteLine("Minimum number of letters? (Example: if word = 4 letters then this would be 4; if lenght of word is unknow we suggest 3 minimum)");
                    _debut = int.Parse(Console.ReadLine());
                    Console.Clear();

                    //max
                    Console.WriteLine("Maximum number of letters?");
                    _fin = int.Parse(Console.ReadLine());
                    _fin = _fin + 1;
                    Console.Clear();

                    double time = Math.Round((Math.Pow(26, passLength) / 6000), MidpointRounding.AwayFromZero);

                    //PID
                    int nProcessID = Process.GetCurrentProcess().Id;
                    Console.Write(("pid: " + nProcessID));

                    //Date
                    Console.WriteLine(" Start time: {0}",DateTime.Now);

                    Console.WriteLine("Password lenght = " + passLength);
                    Console.WriteLine("Estimated tries: {0} -> {1} ms @ 6000t/ms", Math.Pow(26, passLength), time);

                    Thread worker1 = new Thread(new ThreadStart(Worker1));
                    Thread worker2 = new Thread(new ThreadStart(Worker2));
                    Thread worker3 = new Thread(new ThreadStart(Worker3));
                    Thread worker4 = new Thread(new ThreadStart(Worker4));
                    Thread worker5 = new Thread(new ThreadStart(Worker5));
                    Thread worker6 = new Thread(new ThreadStart(Worker6));
                    Thread worker7 = new Thread(new ThreadStart(Worker7));
                    Thread worker8 = new Thread(new ThreadStart(Worker8));
                    Thread logo = new Thread(new ThreadStart(Logo));

                    worker1.Start();
                    worker2.Start();
                    worker3.Start();
                    worker4.Start();
                    worker5.Start();
                    worker6.Start();
                    worker7.Start();
                    worker8.Start();
                    logo.Start();

                    //Starting stopwatch
                    stopwatch.Start();

                    //Post verification algorithm
                    while (true)
                    {
                        if (fini == true)
                        {
                            logo.Abort();
                            countAll = count1 + count2 + count3 + count4 + count5 + count6 + count7 + count8;
                            //Arret du timer et conversion du temps en millisecondes
                            stopwatch.Stop();
                            var elapsed = stopwatch.ElapsedMilliseconds;

                            //Stats et affichage du mot de passe
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("password: " + _crack);
                            Console.ForegroundColor = ConsoleColor.White;
                            if (countAll > 1000)
                            {
                                Console.WriteLine(countAll + " tries in " + elapsed + "ms @ " + countAll / elapsed + "t/ms");
                            }
                            else if (countAll > 1000)
                            {
                                Console.WriteLine("Error: Some threads couldn't initialize in time.");
                            }
                            break;
                        }
                    }
                    Console.Read();
                    break;

                case "2":
                    Console.WriteLine("What is the password to guess?");
                    _password = Console.ReadLine();
                    password = _password;
                    passLength = _password.Length;

                    Console.WriteLine("What is the path of the password file you want to use?\n(ex: 'C:/Users/Username/Desktop/password.txt') **replace '/' by backslashes\n*** If file path is wrong or file doesn't exist, program may crash");
                    string filePath = Console.ReadLine();
                    string[] lignes = System.IO.File.ReadAllLines(filePath);

                    //Starting stopwatch
                    stopwatch.Start();

                    foreach (string line in lignes)
                    {
                        if (password == line)
                        {
                            //Arret du timer et conversion du temps en millisecondes
                            stopwatch.Stop();
                            var elapsed = stopwatch.ElapsedMilliseconds;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("password: {0}", line);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Found in {0}ms", elapsed);
                            break;
                        }
                    }
                    Console.Read();
                    break;

                case "3":

                    Console.WriteLine("Password Minimum letters?");
                    int debut = int.Parse(Console.ReadLine());
                    Console.WriteLine("Password Maximum letters?");
                    int fin = int.Parse(Console.ReadLine());
                    Console.WriteLine("What is the path of the program where to inject?");
                    string program = Console.ReadLine();

                    ExternalProcess = new Process();
                    ExternalProcess.StartInfo.FileName = program;
                    ExternalProcess.Start();
                    char touche = ' ';

                    do
                    {
                        if (Console.KeyAvailable)
                        {
                            touche = Console.ReadKey(true).KeyChar;
                        }
                        else
                        {
                            string crack;
                            int lenght;
                            int i;
                            Random random = new Random();
                            int num;
                            char rand;

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
                                SendKeys.Send(crack);
                                System.Windows.Forms.SendKeys.SendWait("{ENTER}");

                            } while (true);
                        }

                    } while (touche != 'Q');
                    break;

                case "4":
                    Console.WriteLine("What is the path of the program where to inject?");
                    program = Console.ReadLine();
                    Console.Clear();

                    Console.WriteLine("What is the path of the password file you want to use?\n(ex: 'C:/Users/Username/Desktop/password.txt') **replace '/' by backslashes\n*** If file path is wrong or file doesn't exist, program may crash");
                    filePath = Console.ReadLine();
                    lignes = System.IO.File.ReadAllLines(filePath);

                    ExternalProcess = new Process();
                    ExternalProcess.StartInfo.FileName = program;
                    ExternalProcess.Start();

                    foreach (string line in lignes)
                    {
                        SendKeys.Send(line);
                        System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    }
                    Console.Read();
                    break;
            }
        }


        public static void Worker1()
        {
            Console.WriteLine("Thread 1 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count1++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker2()
        {
            Console.WriteLine("Thread 2 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count2++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker3()
        {
            Console.WriteLine("Thread 3 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count3++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker4()
        {
            Console.WriteLine("Thread 4 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count4++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker5()
        {
            Console.WriteLine("Thread 5 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count5++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker6()
        {
            Console.WriteLine("Thread 6 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count6++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker7()
        {
            Console.WriteLine("Thread 7 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count7++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Worker8()
        {
            Console.WriteLine("Thread 8 Initialized");
            string password = _password;
            int debut = _debut;
            int fin = _fin;
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
                count8++;
            } while (crack != password);
            _crack = crack;
            fini = true;
        }

        public static void Logo()
        {
            int currentLineCursor = Console.CursorTop;
            string[] logo = new string[4];
            logo[0] = "─";
            logo[1] = "／";
            logo[2] = "|";
            logo[3] = "＼";

            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    Console.WriteLine(logo[i]);
                    Thread.Sleep(100);
                    ClearLine();
                }
            }
            
        }
    }
}
