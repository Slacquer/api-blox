[1]:https://tools.ietf.org/html/rfc7807

# Response Objects
### In regards to: RFC [**7807**][1] Problem Details

<br>

> **Non paginated OK Request (200)**
```json
{
  "data": {
    "id": 2,
    "firstName": "Slacquer",
    "lastName": McLoven",
    "birthDate": "1971-10-12T00:00:00+00:00",
    "locations": null
  }
}
```

<br>
<br>

> **Paginated (pagination is faked, but structure is real) Ok Request (200)**

```json
{
  "data": [
    {
      "id": 1,
      "firstName": "Slacquer 0",
      "lastName": "McLoven 0",
      "birthDate": "1971-10-12T00:00:00+00:00",
      "locations": null
    },
    {
      "id": 2,
      "firstName": "Slacquer 1",
      "lastName": "McLoven 1",
      "birthDate": "1971-11-12T00:00:00+00:00",
      "locations": null
    },
    {
      "id": 3,
      "firstName": "Slacquer 2",
      "lastName": "McLoven 2",
      "birthDate": "1971-12-12T00:00:00+00:00",
      "locations": null
    }
  ],
  "pagination": {
    "totalCount": 0,
    "next": "https://foo.com/nextPage",
    "previous": "https://foo.com/firstPage"
  }
}

```

<br>
<br>

> **All NON-successful responses follow this structure.**

```json
{
  "error": {
    "detail": "Short detailed summary of the problem",
    "errors": [
      {
        "detail": "Short detailed summary of the problem.",
        "title": "Consistent Title"
      }
    ],
    "instance": "A URI reference that identifies the specific occurrence of the problem ",
    "status": HttpStatus code,
    "title": " A short, human-readable summary of the problem type ",
    "type": " A URI reference that identifies the problem type "
  }
}
```

<br>
<br>

> **POST, validation error example**  
> _**Bad Request (400)**_  
> This example has dynamic bits, “Property” added by developer

```json
{
  "error": {
    "detail": "Please refer to the errors property for additional details",
    "errors": [
      {
        "detail": "The length of 'Last Name' must be at least 5 characters. You entered 1 characters.",
        "property": "LastName",
        "title": "Invalid Property Value"
      },
      {
        "detail": "'First Name' should not be empty.",
        "property": "FirstName",
        "title": "Invalid Property Value"
      }
    ],
    "instance": "/api/qa/people",
    "status": 400,
    "title": "One or more validation errors has occured.",
    "type": "about:blank"
  }
}

```


<br>
<br>

> **NON PRDOUCTION SERVER FAULT STRUCTURE**  
> _**Server Error (500)**_  

```json
{
    "error": {
      "detail": "Please refer to the error property for additional information.",
      "errors": [
        {
          "detail": "Some root level exception",
          "errors": [
            {
              "detail": "Specified argument was out of the range of valid values”
              "title": "Error Details",
              "type": "ArgumentOutOfRangeException"
            }
          ],
          "title": "Please refer to the error property for additional information.",
          "type": "Exception"
        }
      ],
      "instance": "/api/qa/people",
      "referenceId": "1110183247",
      "status": 500,
      "title": "An internal server error has occured.",
      "type": "about:blank"
    }
  }
```

<br>
<br>

> **PRDOUCTION SERVER FAULT STRUCTURE**  
> _**Server Error (500)**_  
> Note that the NON-Production fault would be logged, just not sent back in the response.  This could be done just like NON-prod if used in a public API.

```json
{
    "error": {
      "detail": "Please refer to the error property for additional information.",
      "errors": [
        {
          "detail": "Some root level exception",
          "errors": [
            {
              "detail": "Specified argument was out of the range of valid values”
              "title": "Error Details",
              "type": "ArgumentOutOfRangeException"
            }
          ],
          "title": "Please refer to the error property for additional information.",
          "type": "Exception"
        }
      ],
      "instance": "/api/qa/people",
      "referenceId": "1110183247",
      "status": 500,
      "title": "An internal server error has occured.",
      "type": "about:blank"
    }
  }
```