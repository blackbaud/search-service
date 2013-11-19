using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.ConstituentSearch;
using Search.DataSynchronization;
using Search.DataSynchronization.Loaders;

namespace _TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var postit = new HttpPostIt();
            
            //postit.Post("2");


            //return;
            

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("*************************************************************");
            Console.WriteLine("Engine Started - Wating for command - {0}", DateTime.Now);
            Console.WriteLine("*************************************************************");
            Console.ResetColor();

            var wait = true;

            while (wait)
            {
                var consoleresults = Console.ReadLine().ToLower();

                if (consoleresults == "start sync")
                {
                    Search.DataSynchronization.MainContext.Start(true);
                }
                else if (consoleresults == "start load")
                {
                    Search.DataSynchronization.MainContext.Start(false);
                }
                else if (consoleresults == "search")
                {
                    Console.WriteLine("Enter Search Term:");
                    var searchTerm = Console.ReadLine();
                    var search = new QuerySearchIndex(new Guid("B4FFF883-5A27-4DD5-BE33-06FE08FD5CAA"), searchTerm);
                    search.Execute();
                    Console.WriteLine(search.JsonResults);
                }
                else if (consoleresults == "query")
                {
                    var json = @"{
        'Selections': [
            {
                'CollectionName': 'Constituent',
                'FieldName': '_id'
            },
            {
                'CollectionName': 'Constituent',
                'FieldName': 'KeyNAme'
            },
            {
                'CollectionName': 'Constituent',
                'FieldName': 'FirstName'
            },
            {
                'CollectionName': 'Constituent',
                'FieldName': 'Age'    
            },
            {
                'CollectionName': 'Constituent.Address',
                'FieldName': 'Address.ADDRESSBLOCK'
            },
            {
                'CollectionName': 'Constituent.Address',
                'FieldName': 'Address.CITY'
            },
            {
                'CollectionName': 'Constituent.Address',
                'FieldName': 'Address.StateAbrv'
            }
        ],
        'Filters': [
            {
                'FilterName': 'Address.StateAbrv',
                'Key': ['TX', 'SC'],
                 'Operator': 'IN'
            },
            {
                'FilterName': 'Age',
                'Key': [50,48,35,36],
                 'Operator': 'IN'
            }
        ]
}";

                    var queryProcessor = new QueryProcessor(json);
                    var output = queryProcessor.Execute();

                    Console.WriteLine(output);

                }
            }
        }
    }
}
