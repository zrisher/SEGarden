/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenConquest.ChatCommands {


class Example {

    ChatCommands.Branch commands;

    Example() {

        Branch adminBranch = new Branch(
            "admin",
            "administrative commands",
            "A bunch of long info about the administrative commands",
            100,
            new List<Node> {
                new Command(
                    "concealed",
                    "Lists all concealed entities and info about their state",
                    "Lists all concealed entities and info about their state",
                    (List<String> inputs) => new Notifications.Notice(),
                    new List<String> { "n" }
                ),
                new Command(
                    "conceal", 
                    "Conceals grid number n from the Revealed list",
                    "Conceals grid number n from the Revealed list", 
                    (List<String> inputs) => new Notifications.Notice(),
                    new List<String> { "n" }
                ),
                new Command(
                    "revealed",
                    "Lists all revealed entities and info about their state",
                    "Lists all revealed entities and info about their state",
                    (List<String> inputs) => new Notifications.Notice(),
                    new List<String> { "n" }        
                ),
                new Command(
                    "reveal",
                    "Reveals grid number n from the Concealed list",
                    "Reveals grid number n from the Concealed list",
                    (List<String> inputs) => new Notifications.Notice(),
                    new List<String> { "n" }
                )
            });

        Branch cpsBranch = new Branch(
            "cps",
            "cps commands",
            "A bunch of long info about the Control Points",
            0,
            new List<Node> {
                new Command(
                    "show",
                    "show the CP GPS markers",
                    "This command will show the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),

                new Command(
                    "hide",
                    "hide the CP GPS markers",
                    "This command will hide the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),
            });

        Branch fleetBranch = new Branch(
            "fleet",
            "fleet commands",
            "A bunch of long info about the fleet commands",
            0,
            new List<Node> {
                new Command(
                    "list",
                    "show the CP GPS markers",
                    "This command will show the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),

                new Command(
                    "violations",
                    "hide the CP GPS markers",
                    "This command will hide the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),
                new Command(
                    "stop",
                    "hide the CP GPS markers",
                    "This command will hide the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice(),
                    new List<String> {"n"}
                    ),
            });

        Branch helpBranch = new Branch(
            "help",
            "help info",
            "A bunch of long info about Garden Conquest subtopics",
            0,
            new List<Node> {
                new Command(
                    "show",
                    "show the CP GPS markers",
                    "This command will show the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),

                new Command(
                    "hide",
                    "hide the CP GPS markers",
                    "This command will hide the CP GPS markers",
                    (List<String> inputs) => new Notifications.Notice()
                    ),
            });


        ChatCommands.Branch allCommands = new ChatCommands.Branch(
            "gc",
            "All commands for Garden Conquest",
            "Longer block of info",
            0,
            new List<Node> {
                (Node)adminBranch,
                (Node)cpsBranch,
                (Node)fleetBranch,
                (Node)helpBranch,
            }
        );


    }
}
}

*/