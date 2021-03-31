using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace backup
{
    class Profile //class for managing creation, deletion, unpacking from file and packing profiles as well as instances of them
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

        //static methods
        public static void Unpack(string contents, List<Profile> profiles) //static keyword applies method to general 'Program' class rather than a specific instance
        {
            string[] rawProfiles = contents.Split("\n"); //splits by carriage return

            foreach (string profile in rawProfiles)
            {
                string[] profileValues = profile.Trim().Split(","); //removes newlines from profile names and splits them by comma
                if (profileValues.Length == 3) //if profileValues is the right length
                {
                    Profile tempProfile = new Profile(profileValues[0], profileValues[1], profileValues[2]);//passes csv to constructor 
                    profiles.Add(tempProfile);
                    Console.WriteLine(tempProfile.name); //writes properties to console 
                }
            }
        }

        public static string Repack(List<Profile> profiles)
        {
            string contents = "";

            bool firstPass = true;
            foreach (Profile profile in profiles)
            {
                string tempProfile = profile.getProperties(); //returns a string of all property values separated by commas
                if (firstPass) { contents += $"{tempProfile}"; } //adds it onto the filestream with a carriage return inbetween 
                else { contents += $"\n{tempProfile}"; } //adds a newline all but the first time to not create a blank record at the start
                firstPass = false;
            }

            return contents;
        }

        public static void NewProfile(List<Profile> profiles) //adding profiles
        {
            Console.WriteLine("\n>> create a profile");
            try
            {
                bool loop = true;
                while (loop) //while the user wants to add more profiles
                {
                    bool badName = true;
                    string name = "";

                    while (badName)
                    {//while name is not unique
                        Console.WriteLine("enter name for profile (to escape without creating a new profile, enter 'exit')");
                        name = Console.ReadLine().Trim();

                        if (name == "exit") { break; } //checks if the user wants to exit, breaks out of while loop

                        int count = 0;
                        foreach (Profile profile in profiles){if ((profile.name != name) && (name != "exit")){ count++; }}
                        //if all profiles had different names to the one entered, name is good, else name remains bad and question loops
                        if (count == profiles.Count) { badName = false; } else { Console.WriteLine("you already have a profile with that name"); } 
                    }

                    if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                    {
                        Console.WriteLine("exiting create profile");
                        break;
                    }

                    Console.WriteLine("enter directory to copy from");
                    string sourcePath = Console.ReadLine().Trim();
                    Console.WriteLine("enter directory to copy to");
                    string backupPath = Console.ReadLine().Trim();
                    Profile tempProfile = new Profile(name, sourcePath, backupPath);

                    Console.WriteLine("entered details:");
                    tempProfile.OutputContents();
                    Console.WriteLine($"\nare you sure you want to add '{name}'? (y/n)");

                    if (Console.ReadLine().ToLower().Trim() == "y") //if they enter yes
                    {
                        profiles.Add(tempProfile); //add the profile to list of profiles
                        Console.WriteLine($"sucessfully added '{name}'");
                    }
                    else { Console.WriteLine($"profile '{name}' was discarded"); }

                    Console.WriteLine("enter another profile? (y/n)"); //if the user wants to write a new profile
                    if (Console.ReadLine().ToLower().Trim() != "y") { loop = false; } //if they dont we exit the loop
                }
            }
            catch (FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input not in requested format");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static int LinearSearch(List<Profile> profiles, string name) //linear search for profile of name name
        {
            for (int i = 0; i < profiles.Count; i++) //linear search
            {
                if (profiles[i].name == name)
                {
                    Console.WriteLine("profile found");
                    return i;
                }
            }
            Console.WriteLine("profile not found, try again");
            return -1;
        }

        public static void RemoveProfile(List<Profile> profiles)
        {
            Console.WriteLine("\n>> delete a profile");
            try
            {
                bool loop = true;
                while (loop) //while the user wants to delete profiles
                { 
                    int targetIndex = -1;
                    string name = "";
                    while (true)
                    {
                        Console.WriteLine("enter name of profile (to escape without deleting a profile, enter 'exit')");
                        name = Console.ReadLine().Trim();

                        if (name == "exit") { break; } //checks if the user wants to exit, breaks out of while loop

                        targetIndex = Profile.LinearSearch(profiles, name); //returns index of profile with name name

                        if (targetIndex != -1) { break; }
                    }

                    if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                    {
                        Console.WriteLine("exiting delete profile");
                        break;
                    }
                    Console.WriteLine($"are you sure you want to delete '{name}'? (y/n)");

                    if ((targetIndex != -1) && (Console.ReadLine().ToLower().Trim() == "y")) //removes profile at target index if found (-1 signifies 404)
                    {
                        profiles.RemoveAt(targetIndex);
                        Console.WriteLine($"sucessfully removed '{name}'");
                    }
                    else { Console.WriteLine($"kept profile '{name}'"); }

                    Console.WriteLine("delete another profile? (y/n)"); //if the user wants to delete a new profile
                    if (Console.ReadLine().ToLower().Trim() != "y") { loop = false; } //if they dont we exit the loop
                }
            }
            catch (FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input not in requested format");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void Reset(string path, List<Profile> profiles) //clears config file
        {
            Console.WriteLine("\n>> delete all profiles");
            try
            {
                Console.WriteLine($"are you sure you want to delete {profiles.Count} profile(s)? (y/n)");
                if (Console.ReadLine().ToLower().Trim() == "y") {
                    File.WriteAllText(path, "", Encoding.UTF8); //writes config file as blank
                    profiles.Clear(); //removes all profiles from list
                    Console.WriteLine($"deleted all profiles");
                }
                else { Console.WriteLine("kept all profiles"); }

            }
            catch (FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input not in requested format");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void BackupDialogue(List<Profile> profiles) //method to handle user input around backing files up
        {
            Console.WriteLine("\n>> backup a profile");
            try
            {
                int targetIndex = -1;
                string name = "";
                while (true)
                {
                    Console.WriteLine("enter the profile you wish to backup (to escape without backing up a profile, enter 'exit')");
                    name = Console.ReadLine().Trim();

                    if (name == "exit") { break; } //checks if the user wants to exit, breaks out of while loop

                    targetIndex = Profile.LinearSearch(profiles, name);

                    if (targetIndex != -1) { break; }
                }

                if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                {
                    Console.WriteLine("exiting backup");
                    return;
                }

                Console.WriteLine($"are you sure you want to backup '{name}'? (y/n)");

                if (Console.ReadLine().ToLower().Trim() == "y") 
                {
                    Console.WriteLine("do you want to overwrite and update existing files? (y/n)");
                    bool overwrite = false;
                    if (Console.ReadLine().ToLower().Trim() == "y") { overwrite = true; } //overwrite is set to true 
                    profiles[targetIndex].Backup(overwrite);
                }
                else { Console.WriteLine($"profile '{name}' was discarded"); }
            }
            catch (FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input not in requested format");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        //instance methods
        public void OutputContents()
        {
            Console.Write($">> name: {name}\n>> source path: {sourcePath}\n>> backup location: {backupPath}");
        }

        public void Backup(bool overwrite)
        {
            if(Directory.Exists(sourcePath) && Directory.Exists(backupPath))
            {
                //Console.WriteLine($"copying files from \n{sourcePath}\nto {backupPath}");
                Console.WriteLine("backing up...");
                CreateDirectories();
                Copy(overwrite);
            }
            else 
            { 
                Console.WriteLine("could not backup files as one or more profile directories do not exist");
            }
        }

        //the two methods below are both from stack overflow
        public void CreateDirectories()
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, backupPath));
            }
        }

        public void Copy(bool overwrite)
        {
            //bool noAccessError = true;
            int[] count = {0, 0, 0}; //number of files that could not be backed up due to skipping, then because of access level. third number is total writes
            string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            int noFiles = files.Length;
            foreach (string newPath in files)
            {
                try
                {
                    count[2]++;
                    File.Copy(newPath, newPath.Replace(sourcePath, backupPath), overwrite);
                    Console.Write($"\r{count[2]-(count[0]+count[1])}/{noFiles} files backed up (do not close the program)");
                }catch(IOException e) //catches files that cannot be overwritten due to overwrite mode being false
                {
                    count[0]++;
                    Console.WriteLine($"SKIPPED {newPath.Replace(sourcePath, backupPath)}"); //probably a really bad way to deal with this but it works
                }catch(UnauthorizedAccessException e)
                {
                    count[1]++;
                    //if (noAccessError)
                    //{
                    //    Console.WriteLine("\n>> not a high enough access level for this file! (try running the program as administrator)");
                    //    Console.WriteLine(e);
                    //    noAccessError = false;
                    //}
                }
            }
            Console.WriteLine($"\n>> {count[0]} files were skipped\n>> {count[1]} files could not be backed up due to insufficient permissions (try running the app in administrator mode)");
        }
          
        public string getProperties()
        {
            return $"{name},{sourcePath},{backupPath}";
        }
    }

    class Program
    {
        static void ProfileLoop(List<Profile> profiles, string configPath)
        {
            try{
                bool loop = true;
                while (loop)
                {
                    Console.WriteLine("enter an action (backup/create/delete/reset/exit)");
                    string action = Console.ReadLine().ToLower().Trim(); //input

                    //outcomes
                    if (action == "create") { Profile.NewProfile(profiles); } //create profiles
                    else if (action == "delete") { Profile.RemoveProfile(profiles); } //delete profiles
                    else if (action == "backup") 
                    {
                        if (profiles.Count > 0) { Profile.BackupDialogue(profiles); }
                        else { Console.WriteLine("you have no profiles to backup, try creating one!"); }
                    } //backup profiles
                    else if (action == "reset") { Profile.Reset(configPath, profiles); } //delete all profiles
                    else if (action == "exit") //exit the program
                    {
                        Console.WriteLine("saving and exiting...");
                        loop = false; //no longer loops if user exits
                    }
                    else { Console.WriteLine("invalid action"); } //didnt choose one of the above
                }
            }
            catch (FormatException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input not in requested format");
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("backup utility v0");
            string configPath = @"config.txt";

            //Profile.Reset(configPath); //resets config file

            if (!File.Exists(configPath)) { //if the config file doesnt exist in the right directory
                Console.WriteLine("creating config file");
                File.Create(configPath).Dispose(); //make a new config file, then close it so the next line can access it
            }
            else { Console.WriteLine("config file found"); }

            Console.WriteLine("when adding profiles, make sure to exit using the exit command to save them in the config file");

            string configText = File.ReadAllText(configPath, Encoding.UTF8);

            Console.WriteLine("\ndiscovered profiles:");

            List<Profile> profiles = new List<Profile>(); //makes an empty list of profiles
            Profile.Unpack(configText, profiles); //converts file string to profile objects and adds them to a list
            
            Console.WriteLine($"found {Profile.count} profile(s)\n");

            //editing profiles
            //profiles.Add(new Profile("new record", "path5", "path6"));
            ProfileLoop(profiles, configPath);

            //converts list of profiles to a single string to be written back to config if edited 
            configText = Profile.Repack(profiles);

            //Console.WriteLine($"\nwriting:\n{configText}");
            File.WriteAllText(configPath, configText, Encoding.UTF8);
            Console.WriteLine(">> any profile changes were saved");

            Console.WriteLine("press any key to close this window");
            Console.ReadKey();
        }
    }
}
