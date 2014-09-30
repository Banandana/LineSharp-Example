using System;
using System.Collections.Generic;
using LineSharp;
using LineSharp.Common;

namespace LineSharp_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====Login====");
            Console.Write("Username: ");

            string username = Console.ReadLine();

            Console.Write("Password (NOT MASKED): ");

            string password = Console.ReadLine();

            LineClient line = new LineClient();

            line.OnLogin += new LineClient.LoggedInEvent((Result loginResult) =>
            {
                //Everything worked
                if (loginResult == Result.OK)
                {
                    Console.WriteLine("Authed successfully!");
                    List<Contact> ct = line.GetContacts(line.GetContactIDs());

                    Console.WriteLine("Enter the name of a user you would like to send a 'Hi' to:");
                    string name = Console.ReadLine();

                    for (int i = 0; i < ct.Count; i++)
                    {
                        if (ct[i].Name.Contains(name))
                        {
                            line.SendMessage(ct[i].ID, "Hi!");
                        }
                    }
                }
                //Phone verification needed
                else if (loginResult == Result.REQUIRES_PIN_VERIFICATION)
                {
                    //The user then is required to enter the pin (retrieved from calling
                    string pin = line.Pin;
                    //)

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Verify your account on your mobile device using PIN: " + pin);
                    Console.WriteLine("It times out after about 3 minutes.");
                    Console.ForegroundColor = ConsoleColor.White;

                    //Then call this function, then enter the pin on the mobile device.
                    line.VerifyPin();
                    //WARNING: This function will hang until the pin verifies.
                }
                else
                {
                    Console.WriteLine("Did not auth successfully. Paused.");
                    Console.Read();
                    Environment.Exit(0);
                }


            });

            line.OnPinVerified += new LineClient.PinVerifiedEvent((Result pinVerifiedResult, string verifierToken) =>
            {
                //The pin was verified, or it had timed out???
                if (pinVerifiedResult == Result.OK)
                {
                    //Success. Log in using this. After logging in this way, if there's a certificate, you should
                    //save that somewhere and use it to log in again, because apparently it's nice not to have to 
                    //verify your pin every single time. I'll implement logging in and using a cert later though.

                    line.Login(username, password, verifierToken);
                    // :P
                }

            });

            line.OnReceiveMessage += (o, eventArgs) => Console.WriteLine(eventArgs.Message.Text);

            line.Login(username, password);
            while (true) line.Update();
            //Console.Read();
            //Line.Logout();
        }
    }
}