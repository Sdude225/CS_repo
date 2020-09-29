# CS_repo

**Pavlov Alexandru, FAF-181**<br>
**Dodi Cristian-Dumitru, FAF -181**
### Technologies:
- Language C#
- Framework .NET
### Short description:
Parsing chosen audit file and display a list of desciptions from custom_item<br>
Custom audit is saved in AppData/Local/SBT folder.<br>
After execution,we can see a list of descriptions of different color:
 * red - failed<br>
 * light grey - not found/do not exist<br>
 * dark grey - not implemented yet for that custom_item<br>
 * green - passed
<figure class="video_container">
  <video src="https://utm-my.sharepoint.com/:v:/g/personal/mihail_gavrilita_faf_utm_md/EeSoO7T7GpBIv7tFACDXNu8BvgJnvRLfUYY85w1CP-g3pQ?e=KRMNtU" frameborder="0" allowfullscreen="true"> </video>
</figure>
Main implementation code can be found in Form1.cs, Pareser.cs, Scanner.cs, SamServer.cs
