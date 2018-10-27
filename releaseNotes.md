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
