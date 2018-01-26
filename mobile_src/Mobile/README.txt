------------------------------
FLOADING README FILE
------------------------------

Requires: Windows Mobile 6 Professional, .NET Framework 3.5
Stable release: 
Authors: Cocco Gabriele, Francia Matteo, Galdi Liliana, Lazzeri Nicole, Paganucci Stefano, Poggi Alessandro

::: Description :::

This is an open source project developed inside
a univesitary course (Laboratorio Orientato alle Applicazioni)
by a group of students.
The aim of the application is to interact with the Floading web server
to compile questionaries in mobility.

Features:
- Retrieves questionaries that the user is been invited to fill
- Support for "private" and "public" questionaries
- Shows general results and statistics
- Receives notifications and can synchronize with mail agent and sms service
- Filling off-line
- Saves partial compilations
- Sends results to the server

For more and latest information visit the website

http://floading.codeplex.com/sourcecontrol/changeset/view/22269?projectName=floading

::: Installation :::

1. Connect your device to computer and open Windows Mobile Device Center if it
does not start automatically. 
2. Copy the .cab file into a directory of your device (it doesn't matter which directory you choose, the important is to remember where
you placed it. Now you can disconnect the device).
3. Click the .cab onto your device. The installation starts.
4. Once the installation is finished, just navigate in your Programs directory and enjoy
the software.

::: Configuration :::

If you desire to configure auto-detecting e-mail invitations make sure to
have a Push funtionality active on your mail agent.

::: Usage :::

In the main page you can choose between multiple options: you can download a public or a private
questionary giving informations such as the identifier, username etc.
Once the questionary has been donwloaded you can easily open it clicking on the
exclamation point in the main page. The questionaries downloaded can be filled or deleted from
the local hard disk.
Filling is very simple: just fill the fields on the screen and then press "Next" or "Back" to
correct some previous informations inserted. If you close the window during filling the informations
written until that moment are saved locally.
If you reach the end of the questionary you can send the results to the server and restart
from the main page.
In the main page you can also find "Settings" and "Help".

::: Troubleshooting :::

If the application doesn't start check if a directory named "Data" exists into your
application folder. If not create it and restart the application.
If your device runs Windows Mobile Standard, the application starts but it's unusable
because some features like touch screen and some graphical controls are not able in this
version of Windows Mobile.
We hope we'll fix this problem as soon as possible.

::: Frequently Asked Questions :::

Q1. What if the battery goes down?
A1. The application saves your work so you can continue your filling later.

Q2. Can i save my work when the filling is not still complete?
A2. Yes, by closing the form.

Q3. What does the symbol with the exclamation point means?
A3. Means that you have some questionaries that are not still completed.

Q4. How can i receive notifications about questionaries that i've been invited to fill?
A4. Just open "Settings" and tick "Detect floading sms/mail". The application automatically starts when you receive an invitation.
  
::: Folders :::

Mobile project

- Communication
	Classes for the communication with the web service
- Data
	Contains descriptions and saved data of user questionaries
- Fields
	Implements the abstract fields contained in all possible forms
- Resources
	Images and configuration files
- Settings
	Program settings are managed here
- Utils
	Utility classes such as enhanced controls
- Workflow
	The core of the application. Classes for the interpretation and correctness check of questionaries
- GUI
	Self explanatory

MobileListener project
	Contains classes for the sms and mail synchronization.

Report bugs on

http://floading.di.unipi.it

