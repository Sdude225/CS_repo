# CS_repo

Pavlov Alexandru, FAF-181
Lab work was done in 2, with Dodi Cristian-Dumitru

The programming language used is C#, .NET framework for easy creation of application forms and dialogs.

After parsing, we simply display the list of desciptions from custom_item

We can choose different descriptions from the list and set desired name of the file, it is saved in AppData/Local/SBT folder.

Pressing "Scan" button will scan the system for the choosed items.<br>
Pressing "Apply" compare local configuration with audit and will display following info:
items with red background color - failed <br>
light grey - not found/do not exist <br>
dark grey - not implemented yet for that custom_item <br>
green - passed <br>

Video demonstration of inplemented features: https://youtu.be/rJhRBEcN6Vw

Main implementation code can be found in Form1.cs, Pareser.cs, Scanner.cs, SamServer.cs
