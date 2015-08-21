//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

///<summary>
/// How do we design the simplest way for updates to work but still be really flexible?
///
/// Two types: session and entity
///
/// Initialize - we could just do new, because we know these won't be constructed until initialization. But this seems a little more clear
/// Terminate
/// GetUpdateRegistry - dictionary of resolution, action
/// 
/// Entities simply get passed the entity in initialize or new
/// 
/// Expose the methods in update manager so other scripts can arbitrarily add if they want,
/// but that requires a session hook (which you could actually use from this), so provide
/// attributes too for those that would like to take the simpler approach
///</summary>
namespace SEGarden.Logic {

    public enum RunLocation : byte {
        Unknown,
        Any,
        Client,
        Server,
        Singleplayer,
    }

    public enum RunStatus : byte {
        NotInitialized, 
        Running, 
        Terminated
    }


}
