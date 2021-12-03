using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodMatchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            bool start = true;
            //Asking the user to choose what he wants
            Console.WriteLine("Please enter your selection:");
            Console.WriteLine("1 - reading the input from the console");
            Console.WriteLine("2 - reading the input from a CSV file format\n");
            string choice;
            //Asking for the choice while the user doesn't enters 1 or 2
            do
            {
                choice = Console.ReadLine();
                if (!(choice == "1" || choice == "2")) Console.WriteLine("Invalid input. Please try again!");
            } while (!(choice == "1" || choice == "2"));
            //If the user enterred 1 them we know that he wants to check the match between the persons
            if (choice == "1")
            {
                //Asking for the first persons name
                Console.WriteLine("Please enter the first persons name!");
                MatchResult result = new MatchResult();
                //Checking the users input while he/she enters a valid one and displaying an error message if it's ivalid
                do
                {
                    if (!start) Console.WriteLine("Invalid input. Please try again!");
                    start = false;
                    result.FirstName = Console.ReadLine();
                } while (!IsInputValid(result.FirstName));
                //Asking for the second persons name
                Console.WriteLine("Please enter the second persons name!");
                start = true;
                //Checking the users input while he/she enters a valid one once again and displaying a message if it is invalid
                do
                {
                    if (!start) Console.WriteLine("Invalid input. Please try again!");
                    start = false;
                    result.SecondName = Console.ReadLine();
                } while (!IsInputValid(result.SecondName));
                StringBuilder sb = new StringBuilder();
                //Bulding the {name1} matches {name2} string
                sb.Append(result.FirstName + " matches " + result.SecondName);
                //Getting the percentage
                result.Percentage = GetPercentage(sb.ToString());
                //Saving the result to the file
                SaveToFile(new List<MatchResult> { result });
            }
            else
            {
                //Asking for the number of people
                Console.WriteLine("How many people do you want to enter?");
                List<InputClass> list = new List<InputClass>();
                int number;
                start = true;
                //Getting the number of people's data that the user wants to enter
                do
                {
                    if (!start) Console.WriteLine("Invalid input. Please try again!");
                    start = false;
                    try
                    {
                        number = Convert.ToInt32(Console.ReadLine());
                    }
                    catch { number = 0; }
                }
                while (number <= 0);
                //Asking the user to enter the data, hopefully in csv format
                Console.WriteLine("Please enter the data in CSV format:");
                for (int i = 0; i < number; i++)
                {
                    string[] arr = Console.ReadLine().Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    //Splitting the input and adding the data to a list
                    if (arr.Length == 2 && !list.Any(x => x.Name == arr[0].Trim() && x.Gender == arr[1].Trim())) list.Add(new InputClass { Name = arr[0].Trim(), Gender = arr[1].Trim() });
                }
                List<MatchResult> results = new List<MatchResult>();
                //Selecting all male person's from the list
                foreach (InputClass male in list.Where(x => x.Gender.ToLower() == "m"))
                {
                    //Selecting all female person's from the list
                    foreach (InputClass female in list.Where(x => x.Gender.ToLower() == "f"))
                    {
                        //Matching them so that every male person is mathed with every female person
                        results.Add(new MatchResult
                        {
                            FirstName = male.Name,
                            SecondName = female.Name,
                            Percentage = GetPercentage(male.Name + " matches " + female.Name)
                        });
                    }
                }
                //Saving the result to the output.txt file
                SaveToFile(results);
                //Displaying a message to the user that the execution has finished
                Console.WriteLine("\nResults saved to output.txt");
            }
            //Waiting for a users input to close the console
            Console.ReadLine();
        }

        static int GetPercentage(string input)
        {
            //Converting the input to lower
            input = input.ToLower();
            List<KeyValuePair<char, int>> list = new List<KeyValuePair<char, int>>();
            //Counting every letter in the input
            foreach (char c in input)
            {
                if (!list.Any(x => x.Key == c) && !char.IsWhiteSpace(c))
                {
                    list.Add(new KeyValuePair<char, int>(c, input.Count(x => x == c)));
                }
            }
            StringBuilder sb = new StringBuilder();
            //Adding the number to a string
            foreach (KeyValuePair<char, int> kvp in list) sb.Append(kvp.Value);
            //Saving the number
            string oldResult = sb.ToString();
            do
            {
                sb.Clear();
                //Looping while the starting number is not empty
                while (oldResult.Length != 0)
                {

                    if (oldResult.Length == 1)
                    {
                        //if there is only one item left in the string we are adding it to the string builder
                        sb.Append(oldResult[0]);
                        oldResult = "";
                    }
                    else
                    {
                        //Adding up the most left and most right numbers and adding the the string builder
                        sb.Append(Convert.ToInt32(oldResult.First().ToString()) + Convert.ToInt32(oldResult.Last().ToString()));
                        oldResult = oldResult.Remove(oldResult.Length - 1, 1);
                        oldResult = oldResult.Remove(0, 1);
                    }
                }
                //Saving the string builders result to a string variable
                oldResult = sb.ToString();
                //Checking if we have a 2 digit number, if not then we are going again
            } while (sb.ToString().Length > 2);
            //Returning the percentage
            if (sb.ToString().Length == 1) return Convert.ToInt32(sb.ToString()[0].ToString());
            return Convert.ToInt32(sb.ToString());
        }

        public static void SaveToFile(List<MatchResult> list)
        {
            Console.WriteLine();
            //appending the results to the output.txt file
            using (StreamWriter sw = new StreamWriter("output.txt", true))
            {
                StringBuilder sb = new StringBuilder();
                foreach (MatchResult result in list.OrderByDescending(x => x.Percentage).ThenBy(x => x.FirstName + " matches " + x.SecondName))
                {
                    sb.Clear();
                    sb.Append(result.FirstName + " matches " + result.SecondName);
                    sb.Append(" " + result.Percentage + "%");
                    if (result.Percentage >= 80) sb.Append(", good match");
                    //writing the result to the text file
                    sw.WriteLine(sb.ToString());
                    //displaying the result on the console
                    Console.WriteLine(sb.ToString());
                }
            }
        }

        public static bool IsInputValid(string input)
        {
            //Checking if the input contains only letters or not
            foreach (char c in input.ToLower()) if (!char.IsLetter(c)) return false;
            return true;
        }
    }
}
