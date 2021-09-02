﻿using System;
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
                    //Console.WriteLine(tempProfile.name); //writes properties to console 
                }
            }

            showProfiles(profiles);
        }

        public static void showProfiles(List<Profile> profiles)
        {
            Console.WriteLine($"\n>> discovered {profiles.Count} profiles");

            foreach(Profile profile in profiles)
            {
                Console.WriteLine($"'{profile.name}', {profile.sourcePath} -> {profile.backupPath}"); //writes properties to console 
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

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        name = Console.ReadLine().Trim();
                        Console.ForegroundColor = ConsoleColor.White;

                        if (name == "exit") { break; } //checks if the user wants to exit, breaks out of while loop

                        int count = 0;
                        foreach (Profile profile in profiles){if ((profile.name != name) && ((name != "exit")&&(name != "all"))){ count++; }}
                        //if all profiles had different names to the one entered, name is good, else name remains bad and question loops
                        if (count == profiles.Count) { badName = false; } else { Console.WriteLine("bad profile name (you may already have a profile with that name)"); } 
                    }

                    if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                    {
                        Console.WriteLine("exiting create profile");
                        break;
                    }

                    Console.WriteLine("enter directory to copy from");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string sourcePath = Console.ReadLine().Trim();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("enter directory to copy to");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string backupPath = Console.ReadLine().Trim();
                    Console.ForegroundColor = ConsoleColor.White;
                    Profile tempProfile = new Profile(name, sourcePath, backupPath);

                    Console.WriteLine("entered details:");
                    tempProfile.OutputContents();
                    Console.WriteLine($"\nare you sure you want to add {name}? (y/n)");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (Console.ReadLine().ToLower().Trim() == "y") //if they enter yes
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        profiles.Add(tempProfile); //add the profile to list of profiles
                        Console.WriteLine($"sucessfully added {name}");
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"profile {name} was discarded"); 
                    }

                    Console.WriteLine("enter another profile? (y/n)"); //if the user wants to write a new profile

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (Console.ReadLine().ToLower().Trim() != "y") { loop = false; } //if they dont we exit the loop
                }
            }
            catch (FormatException e){Formatting(e);}
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

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        name = Console.ReadLine().Trim();
                        Console.ForegroundColor = ConsoleColor.White;

                        if (name == "exit") { break; } //checks if the user wants to exit, breaks out of while loop

                        targetIndex = Profile.LinearSearch(profiles, name); //returns index of profile with name name

                        if (targetIndex != -1) { break; }
                    }

                    if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                    {
                        Console.WriteLine("exiting delete profile");
                        break;
                    }
                    Console.WriteLine($"are you sure you want to delete {name}? (y/n)");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if ((targetIndex != -1) && (Console.ReadLine().ToLower().Trim() == "y")) //removes profile at target index if found (-1 signifies 404)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        profiles.RemoveAt(targetIndex);
                        Console.WriteLine($"sucessfully removed {name}");
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"kept profile {name}");
                    }

                    Console.WriteLine("delete another profile? (y/n)"); //if the user wants to delete a new profile

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (Console.ReadLine().ToLower().Trim() != "y") { loop = false; } //if they dont we exit the loop
                }
            }
            catch (FormatException e){Formatting(e);}
        }

        public static void Reset(string path, List<Profile> profiles) //clears config file
        {
            Console.WriteLine("\n>> delete all profiles");
            try
            {
                Console.WriteLine($"are you sure you want to delete {profiles.Count} profile(s)? (y/n)");

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (Console.ReadLine().ToLower().Trim() == "y") {

                    Console.ForegroundColor = ConsoleColor.White;
                    //File.WriteAllText(path, "", Encoding.UTF8); //writes config file as blank
                    profiles.Clear(); //removes all profiles from list
                    Console.WriteLine($"deleted all profiles");
                }
                else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("kept all profiles"); 
                }

            }
            catch (FormatException e){Formatting(e);}
        }

        public static void Backup(List<Profile> profiles) //method to handle user input around backing files up
        {
            Console.WriteLine("\n>> back up a profile");
            try
            {
                int targetIndex = -1;
                string name = "";
                while (true)
                {
                    Console.WriteLine("enter the profile you wish to back up (to escape without backing up a profile, enter 'exit')\nif you wish to back up all your profiles, enter 'all'");
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    name = Console.ReadLine().Trim();
                    Console.ForegroundColor = ConsoleColor.White;

                    if ((name == "exit")||(name == "all")) { break; } //checks if the user wants to exit or backup all profiles, breaks out of while loop

                    targetIndex = Profile.LinearSearch(profiles, name);

                    if (targetIndex != -1) { break; }
                }

                if (name == "exit") //checks if the user wants to exit, breaks out of while loop
                {
                    Console.WriteLine("exiting backup");
                    return;
                }

                Console.WriteLine($"are you sure you want to back up {name}? (y/n)");

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (Console.ReadLine().ToLower().Trim() == "y") 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("do you want to overwrite and update existing files? (y/n)");
                    bool overwrite = false;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (Console.ReadLine().ToLower().Trim() == "y") { overwrite = true; } //overwrite is set to true 
                    if (targetIndex != -1) { profiles[targetIndex].Backup(overwrite); }
                    else
                    {
                        foreach(Profile profile in profiles)
                        {
                            profile.Backup(overwrite);
                        }
                    }
                }
                else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{name} not backed up");
                }
            }
            catch (FormatException e){Formatting(e);}
        }
        public static void Formatting(FormatException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("input not in requested format");
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Permission(UnauthorizedAccessException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("unauthorised access at this level of permission");
            Console.WriteLine(e);
            Console.WriteLine("try running the program as admin");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        //instance methods
        public void OutputContents()
        {
            Console.Write($">> name: {name}\n>> source path: {sourcePath}\n>> backup location: {backupPath}");
        }

        public void Backup(bool overwrite)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"backing up {name}...");
            if (Directory.Exists(sourcePath) && Directory.Exists(backupPath))
            {
                //Console.WriteLine($"copying files from \n{sourcePath}\nto {backupPath}");
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
            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, backupPath));
                }
            }
            catch (UnauthorizedAccessException e){Permission(e);}
        }

        public void Copy(bool overwrite)
        {
            int[] count = { 0, 0, 0 }; //number of files that could not be backed up due to skipping, then because of access level. third number is total writes
            try
            {
                //bool noAccessError = true;
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
                        Console.WriteLine($">> skipped {newPath.Replace(sourcePath, backupPath)}"); //probably a really bad way to deal with this but it works
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
            }catch(UnauthorizedAccessException e){Permission(e);}
            
            Console.WriteLine($"\n>> {count[0]} files were skipped\n>> {count[1]} files could not be backed up due to insufficient permissions");
        }
          
        public string getProperties()
        {
            return $"{name},{sourcePath},{backupPath}";
        }
    }

    class Program
    {
        static bool ProfileLoop(List<Profile> profiles, string configPath)
        {
            try{
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nenter action (enter 'help' for actions)");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string action = Console.ReadLine().ToLower().Trim(); //input
                    Console.ForegroundColor = ConsoleColor.White;

                    //outcomes
                    if (action == "help")
                    {
                        Console.WriteLine("\n>> actions: \n" +
                            "help - bring up this dialogue\n" +
                            "profiles - show all profiles saved to config\n" +
                            "backup - copy files according to a specified profile\n" +
                            "create - set up a new profile\n" +
                            "delete - remove a profile\n" +
                            "reset - delete ALL profiles\n" +
                            "exit - save and exits\n" +
                            "exit w/o saving - exit the program WITHOUT saving changes made in a session");
                    }
                    else if (action == "profiles") { Profile.showProfiles(profiles); }
                    else if (action == "create") { Profile.NewProfile(profiles); } //create profiles
                    else if (action == "delete") { Profile.RemoveProfile(profiles); } //delete profiles
                    else if (action == "backup") 
                    {
                        if (profiles.Count > 0) { Profile.Backup(profiles); }
                        else { Console.WriteLine("you have no profiles to back up, try creating one!"); }
                    } //backup profiles
                    else if (action == "reset") { Profile.Reset(configPath, profiles); } //delete all profiles
                    else if (action == "exit") //exit the program
                    {
                        Console.WriteLine("\n>> saving...");
                        return true;
                    }
                    else if(action == "exit w/o saving")
                    {
                        Console.WriteLine("\n>> exiting...");
                        return false;
                    }
                    else { Console.WriteLine("invalid action"); } //didnt choose one of the above
                }
            }
            catch (FormatException e)
            {
                Profile.Formatting(e);
                return true;
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

            List<Profile> profiles = new List<Profile>(); //makes an empty list of profiles
            Profile.Unpack(configText, profiles); //converts file string to profile objects and adds them to a list

            //editing profiles
            //profiles.Add(new Profile("new record", "path5", "path6"));
            bool save = ProfileLoop(profiles, configPath);

            //converts list of profiles to a single string to be written back to config if edited 
            if (save)
            {
                configText = Profile.Repack(profiles);
                File.WriteAllText(configPath, configText, Encoding.UTF8);
                Console.WriteLine("profile changes saved");
            }
            else { Console.WriteLine("profile changes not saved");}

            Console.WriteLine("press any key to close this window");
            Console.ReadKey();
        }
    }
}
