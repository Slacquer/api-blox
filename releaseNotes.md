_October 27th, 2018_  **v1.0.69**
- A simple pagination issue,  
...The contract reolver was not being injected for alias mapping, therefore when 
pagination entries came in the request, unless they were an exact match IE: SKIP not 
$SKIP, then they were not being auto populated...
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.68**
- A fairly significant bug - continues, **again**   
...After some soul searching I realized I was just bandaiding things, so I stopped took a breath and re-wrote most of the ServerFaultsMiddleware....
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.49**
- A fairly significant bug - continues  
...Same as below, but it has been bugging me for a while, I didnt think any throwing should be going 
on while the middleware was doing its thing.  So I have introduced a NoThrow protected internal property 
to allow the faults middlware to tun OFF throwing....
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 27th, 2018_  **v1.0.48**
- A fairly significant bug.  
...*ServerFaultsMiddleware* sets errors to null when NOT in development (after logging) to prevent consumers from seeing too much.  However, the  
Base objects were doing a Errors.Any() and not checking for null first.  Therefore in prod, NOTHING would 
get returned to consumer.  This is nasty and requires a build...
 
Thanks,  
_Slacquer_  

<br>
<br>

_October 23th, 2018_  **v1.0.47**
- A few minor bug fixes have been applied.  
- Also updated the examples, with more still to come.  
 
Thanks,  
_Slacquer_
