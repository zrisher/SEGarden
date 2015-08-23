# SEGarden
SE Garden is a collection of helper libraries that provide a layer of abstraction 
over ModAPI for common Space Engineers modding tasks. Some useful features include:

* Full chat command management, with argument handling, error handling, and an 
intelligent tree-traversal system that automatically generates documentation
and usage error messages.

* Full automatic file management, with file handler management and simplified interfaces
for reading and writing to files.

* A highly customizable self-managed logger class

* EntityComponents and SessionComponents that wait for initialization until SE and
the game are ready and can be registered with completely custom conditions and 
update resolutions.

* A large number of generally useful extensions over SE objects.

# Usage
Install SEGarden as a [git submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules)
 within your project. 

When developing, you shouldn't have any issues including it as-is into your project,
as long as it lives *at the same level or below* the highest level file that
declares any namespace referencing it

When deploying, you will need to use a build tool that compresses script directories
together, due to limitations in the way mods are packaged for steam (folders past the
second level are no longer found). I highly recommend [SE Mod Builder](https://github.com/zrisher/se_mod_builder),
based off the builder for Rynchodon's excellent mod [Autopilot](https://github.com/rynchodon/autopilot).

# Contributing
Contributions are very welcome!