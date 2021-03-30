using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace backup
{
    class Profile
    {
        private string sourcePath; //file/folder which is being copied from 
        private string backupPath; //file/folder which its contents get pasted to 
        public string name; //name of profile

        public static int count; //number of profiles
        
        public Profile(string _name, string _sourcePath, string _backupPath) //constructor
        {
            name = _name;
            sourcePath = _sourcePath;
            backupPath = _backupPath;
            count++;
        }

        public void OutputContents()
        {
            Console.WriteLine($"name: {name}, source path: {sourcePath}, backup location: {backupPath}");
        }
        public string getProperties()
        {
            return $"{name},{sourcePath},{backupPath}";
        }
    }

    class Program
    {
        public static void Unpack(string contents, List<Profile> profiles) //static keyword applies method to general 'Program' class rather than a specific instance
        {
            string[] rawProfiles = contents.Split("\n"); //splits by carriage return

            foreach(string profile in rawProfiles)
            {
                string[] profileValues = profile.Trim().Split(","); //removes newlines from profile names and splits them by comma
                if (profileValues.Length != 1)
                {
                    Profile tempProfile = new Profile(profileValues[0], profileValues[1], profileValues[2]);//passes csv to constructor 
                    profiles.Add(tempProfile);
                    tempProfile.OutputContents(); //writes properties to console 
                }
            }
        }

        public static string Repack(List<Profile> profiles)
        {
            string contents = "";
            
            bool firstPass = true;
            foreach(Profile profile in profiles)
            {
                string tempProfile = profile.getProperties(); //returns a string of all property values separated by commas
                if (firstPass) { contents += $"{tempProfile}"; } //adds it onto the filestream with a carriage return inbetween 
                else { contents += $"\n{tempProfile}"; } //adds a newline all but the first time to not create a blank record at the start
                firstPass = false;
            }

            return contents;
        }

        public static void Reset(string path)
        {
            File.WriteAllText(path, "", Encoding.UTF8);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("backup utility v0");
            //Profile profile = new Profile(@"c:\",@"d:\","bob");
            string configPath = @"config.txt";
            //Reset(configPath);
            if (!File.Exists(configPath)) { //if the config file doesnt exist in the right directory
                Console.WriteLine("creating config file");
                File.Create(configPath).Dispose(); //make a new config file, then close it so the next line can access it
            }
            else { Console.WriteLine("config file found"); }

            string configText = File.ReadAllText(configPath, Encoding.UTF8);

            Console.WriteLine("\ndiscovered profiles:");

            List<Profile> profiles = new List<Profile>(); //makes an empty list of profiles
            Unpack(configText, profiles); //converts file string to profile objects and adds them to a list
            
            Console.WriteLine($"{Profile.count} profiles in total");

            //editing profiles
            profiles.Add(new Profile("new record", "path5", "path6"));

            //converts list of profiles to a single string to be written back to config if edited 
            configText = Repack(profiles);

            Console.WriteLine($"\nwriting:\n{configText}");
            File.WriteAllText(configPath, configText, Encoding.UTF8);

            Console.ReadKey();
        }
    }
}
