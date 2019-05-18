# TeslaCamBrowser

This project is 3 seperate parts that work together.

1) TeslaSync 
  Designed to be ran on a raspberry pi 3. This will rsync saved clips from dashcam/sentry mode to a specificed directory 
  and also rsync music from a specified directory to a usb stick.
  
  Requirements:
    a) dependency: automount
    b) the music partition of the usb stick must have a file named 'music'
    c) replace the variables at the top of the teslasync file with your environment
    d) set teslasync to run either via a systemd service or /etc/rc.local



2) TeslaMerge
  This will merge front/left/right camera recordings into a single file, it will also create a preview file of lower
  resolution and a timelapse file for quicker viewing
  
  Requirements:
    a) dependency: ffmpeg
    b) replace the variables at the top of the teslasync file with your environment
    
    
    
3) TeslaCamBrowser
  This is a .net core website designed to view the output of above. The link generated from the link file can be viewed
  without authing; so it's suitible to share with others should you desire
    
  Requirements:
    a) dependency: aspnet core; nginx (or another reverse proxy)
    b) replace the path in appsettings.json 
    c) the default account is: admin\Tesl@Cam21 after loggin in, you should click on admin in upper right and change the password
