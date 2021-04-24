using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Media;
using System.Diagnostics;

namespace BadApple
{
    class Program
    {
        static void Main(string[] args)
        {
            //Audio
            SoundPlayer sound = GetAudio();
            Console.WriteLine("Rendering...");
            //render
            string[] footage = render();
            //string print = "";
            //string lines = "";
            /*for (int width = 0; width <= footage.GetUpperBound(1); width += 2)
            {
                lines += "\n";
            }*/
            //setup
            float framerate = 33;
            bool validAns = false;
            bool NotWentOnce = true;
            //framerate
            if (footage.GetUpperBound(0) > 0)
            {
                string frameText = "";
                while (!validAns)
                {
                    if (NotWentOnce)
                        Console.WriteLine("what framerate would you like it to be displayed as");
                    else
                        Console.WriteLine("Please input a valid number");
                    frameText = Console.ReadLine();
                    validAns = float.TryParse(frameText, out framerate) && frameText != "";
                }
                
                if (framerate <= 0)
                    framerate = float.MaxValue;
                else
                    framerate = (1 / framerate * 1000);
            }
            //SaveDialogue
            bool ans = false;
            string InputText = "";
            if (NotWentOnce)
                Console.WriteLine("would you like to save this to a text file (y)");
            else
                Console.WriteLine("");
            InputText = Console.ReadLine();
            ans = InputText.ToLower() == "y";
            
            if (ans)
            {
                SaveAnim(footage);
            }
            //prep user
            Console.WriteLine("Ready to Start, remember to zoom out");
            Console.ReadKey(true);
            //print = "";
            //clear setup from view
            //for (int i = 0; i <= 200; i++)
            //{
            //    Console.Write("\n");
            //}
            //play audio
            //print
            Stopwatch sw = new Stopwatch();
            int overFlow = 0;
            sw.Start();
            if (sound != null)
            {
                sound.Play();
            }
            for (int frame = 0; frame <= footage.GetUpperBound(0); frame++)
            {
                //sw.Start();
                //for(int width = 0; width<= footage.GetUpperBound(1); width+=resDivider)
                //{
                //    for (int height = 0; height <= footage.GetUpperBound(0); height+=resDivider)
                //    {
                //        print += footage[height, width, frame];
                //    }
                //    print += "\n";
                //}
                //print += lines;
                Console.WriteLine(footage[frame]);
                if (frame == 3000)
                {
                    framerate += 0.6f;
                }
                //print = "";
                //if ((int)sw.ElapsedMilliseconds + overFlow + 6 < framerate)
                //{
                //    Thread.Sleep(Convert.ToInt32(framerate - sw.ElapsedMilliseconds + overFlow));
                //    overFlow = 0;
                //}
                //else
                //{
                //    overFlow = (int)(framerate - sw.ElapsedMilliseconds + overFlow);
                //}
                while (sw.ElapsedMilliseconds + overFlow < framerate) ;
                overFlow = 0;
                //if (sw.ElapsedMilliseconds > framerate)
                //    overFlow = (int)(framerate - sw.ElapsedMilliseconds);
                sw.Restart();
                //Console.ReadKey(true);
            }
            //finish
            Console.ReadKey(true);
        }

        /// <summary>
        /// Renders footage
        /// </summary>
        /// <returns></returns>
        static string[] render()
        {
            string[,,] RenderedFootage;
            //grab files
            string[] files = Directory.GetFiles("BadAppleFiles");
            int totalFrames = 0;
            //get resolution
            int[] resolution = new int[2];
            for (int i = 0; i <= files.GetUpperBound(0); i++)
            {
                if (files[i].EndsWith(".png") || files[i].EndsWith(".JPG") || files[i].EndsWith(".PNG") || files[i].EndsWith(".JPG"))
                {
                    totalFrames++;
                    Image image = Image.FromFile(files[i]);
                    resolution = new int[] { image.Width, image.Height };
                }
            }
            RenderedFootage = new string[resolution[0], resolution[1], totalFrames];
            //render all frames
            int BetweenFrameCount = 0;
            Console.WriteLine("FINDING FILES, PLEASE WAIT");
            for (int i = 0; i <= files.GetUpperBound(0); i++)
            {
                if (files[i].EndsWith(".png") || files[i].EndsWith(".JPG") || files[i].EndsWith(".PNG") || files[i].EndsWith(".JPG"))
                {
                    string[,] frame = Conversion(files[i]);
                    for (int n = 0; n <= frame.GetUpperBound(1); n++)
                    {
                        for (int j = 0; j<= frame.GetUpperBound(0); j++)
                        {
                            RenderedFootage[j, n, i - BetweenFrameCount] = frame[j, n];
                        }
                    }
                }
                else
                {
                    BetweenFrameCount++;
                }
            }
            string[] finalRenders = RenderCompiler(RenderedFootage);
            return finalRenders;
        }

        /// <summary>
        /// Does the final compile for the imagess
        /// </summary>
        /// <param name="footage"></param>
        /// <returns></returns>
        static string[] RenderCompiler(string[,,] footage)
        {
            bool validAns = false;
            bool NotWentOnce = true;
            int resDivider = 0;
            int smallerAxis = 0;
            //sets smallest axis
            if (footage.GetUpperBound(0) < footage.GetUpperBound(1))
                smallerAxis = footage.GetUpperBound(0);
            else
                smallerAxis = footage.GetUpperBound(1);
            //Skip lines
            while (!validAns)
            {
                string resText = "";
                if (NotWentOnce)
                    Console.WriteLine($"would you like to skip any lines (input a number between 1 and {smallerAxis - 1})");
                else
                    Console.WriteLine($"Please input a valid number between 1 and {smallerAxis - 1}");
                resText = Console.ReadLine();
                validAns = resText.All(char.IsNumber) && resText != "";
                if (validAns)
                {
                    resDivider = Convert.ToInt32(resText);
                    if (resDivider <= 0 || resDivider >= smallerAxis)
                        validAns = false;
                }
                else if (resText == "")
                {
                    resDivider = 1;
                    validAns = true;
                }
            }
            string print = "";
            string[] CompiledFootage = new string[footage.GetUpperBound(2) + 1];
            for (int frame = 0; frame <= footage.GetUpperBound(2); frame++)
            {
                
                for (int width = 0; width <= footage.GetUpperBound(1); width += resDivider)
                {
                    for (int height = 0; height <= footage.GetUpperBound(0); height += resDivider)
                    {
                        print += footage[height, width, frame];
                    }
                    print += "\n";
                }
                CompiledFootage[frame] = print;
                Console.WriteLine($"rendered frame {frame} of {footage.GetUpperBound(2)}");
                print = "";
            }
            return CompiledFootage;
        }

        /// <summary>
        /// Converts Each image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static string[,] Conversion(string file)
        {
            //get image
            Image image = Image.FromFile(file);
            Bitmap frame = new Bitmap(image);
            string[,] PrintImage = new string[frame.Width, frame.Height];
            Console.WriteLine(file);
            //convert image
            for (int i = 0; i < frame.Width; i++)
            {
                for (int n = 0; n < frame.Height; n++)
                {
                    Color pixel = frame.GetPixel(i, n);
                    //switch (pixel.GetBrightness())
                    //{
                    //    case 1:
                    //        PrintImage[i, n] = Convert.ToChar("@");
                    //        Console.Write(pixel.GetBrightness());
                    //        break;
                    //    default:
                    //        Console.WriteLine(pixel.GetBrightness());
                    //        PrintImage[i, n] = Convert.ToChar(" ");
                    //        break;
                    //}
                    //grab pixel
                    if (pixel.GetBrightness() > 0.8)
                    {
                        PrintImage[i, n] = "@@";
                    }
                    else if (pixel.GetBrightness() > 0.7)
                    {
                        PrintImage[i, n] = "%%";
                    }
                    else if (pixel.GetBrightness() > 0.5)
                    {
                        PrintImage[i, n] = "$$";
                    }
                    else if(pixel.GetBrightness() > 0.3)
                    {
                        PrintImage[i, n] = "##";
                    }
                    else if (pixel.GetBrightness() > 0.1)
                    {
                        PrintImage[i, n] = "!!";
                    }
                    else if (pixel.GetBrightness() > 0.05)
                    {
                        PrintImage[i, n] = "::";
                    }
                    else
                    {
                        PrintImage[i, n] = "  ";
                    }
                }
            }
            //Console.ReadKey(true);
            return PrintImage;
        }

        /// <summary>
        /// gets sound
        /// </summary>
        /// <returns></returns>
        static SoundPlayer GetAudio()
        {
            string[] files = Directory.GetFiles("BadAppleFiles");
            SoundPlayer sound = null;
            for (int i = 0; i <= files.GetUpperBound(0); i++)
            {
                if (files[i].EndsWith(".wav"))
                {
                    sound = new SoundPlayer();
                    sound.SoundLocation = files[i];
                    sound.Load();
                    return sound;
                }
            }
            return null;
        }

        /// <summary>
        /// saves footage
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="resDivider"></param>
        static void SaveAnim(string[] anim)
        {
            StreamWriter swrWrite = new StreamWriter("BadAppleFiles/LastSavedAnim.txt");

            string print = "";
            for (int frame = 0; frame <= anim.GetUpperBound(0); frame++)
            {
                print = $"Frame {frame}\n";
                print += anim[frame];
                
                print += $"\n\n\n\n\n\n\n\n\n";
                Console.WriteLine($"Saved Frame {frame} of {anim.GetUpperBound(0)}");
                swrWrite.Write(print);
            }
            swrWrite.Close();
        }
    }
}
