# CountdownForStreams
Stream Challenge countdown software made with C#/WPF.
<h3>Usage</h3>
<p><b>Working with the timer</b></p>
![image](https://user-images.githubusercontent.com/74548230/130964718-09e69a58-740b-4a3b-b7d5-c6c97c432d5c.png)
<p>Starting the timer is really self explanatory. Select the time format you want to use for the timer. After that write in the time you want to countdown from. Press start :)</p>
<p>To stop the timer simply press the stop button. If you want to reset everything press reset. </p>
<p><b>Adding the timer to OBS</b></p>
<p>1. Open obs and add a new text source like this</p>
![image](https://user-images.githubusercontent.com/74548230/130965175-fb54f343-735f-4143-b883-49857e18e026.png)
<p>2. After adding the text source right click on it and select properties. In the properties there should be a checkbox "Read from a file". Click that and a box to select a file should appear. </p>
![image](https://user-images.githubusercontent.com/74548230/130965539-5d8a5cc1-7dd0-414b-94b5-84effaeb8af7.png)
<p>3. Click the browse button and find the folder where you have the Countdown.exe installed. Now when you start the timer there should appear a folder named "txt. Inside there should be a "Time.txt" file. Select that file and press "Open". Now the timer should be shown in the text. Press OK and there you have it!
![image](https://user-images.githubusercontent.com/74548230/130966115-00f3e6af-b72e-480a-beb5-6fa552e6885d.png)
<p><b>Markup stream highlights easily!</b></p>
<p>When you have the timer running press the HOME button to markup the current time of the counter. It captures the keystroke globally so it works while you are playing any games thanks to Larry57:s repository here: https://gist.github.com/Larry57/5365740. If you still want to check that the markup worked please open the Timestamp Log which shows the last time a markup was taken. After you are done with the stream you can check the marked up times in the txt folder which we looked at in the Obs timer setup. The txt file is named "Markup.txt".</p> 
<p><b>Happy streaming :)</b></p>
