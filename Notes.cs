using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden {
    class Notes {

        // TODO: Server vs Client vs Singleplayer logic
        // various classes for each? Handle it all from here?
        // Until GardenSession is managed by update manager, we'd have to have 3 
        // registered sessions if we wanted to subclass it. So manage from one for now
        // TODO: Message processing in SEGarden

        // TODO: Maybe best way for mod to init:
        // * Have one static session class
        // * Hold logic components within it as static variables initialized on compile
        // * The constructors for those components determine whether they should run 
        //   on the client, server, or both
        //

    }
}
